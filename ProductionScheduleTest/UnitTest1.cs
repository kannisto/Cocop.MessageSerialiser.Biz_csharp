//
// Please make sure to read and understand the files README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 5/2019
// Last modified: 4/2020

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SXml = System.Xml;
using CMeas = Cocop.MessageSerialiser.Meas;
using Cocop.MessageSerialiser.Biz;
using XsdNs = Cocop.MessageSerialiser.Biz.XsdGen;
using XNeut = Cocop.MessageSerialiser.Biz.Neutral;

namespace ProductionScheduleTest
{
    [TestClass]
    public class UnitTest1
    {
        // Testing all classes of the production schedule together in this test.

        
        [TestMethod]
        public void ProcProdSched_Read()
        {
            // Testing reading a regular XML file with all supported features included.

            var filepath = TestCommon.TestHelper.TestFileFolder + "\\ProcessProductionSchedule.xml";
            var xmlBytes = System.IO.File.ReadAllBytes(filepath);

            var testObject = new ProcessProductionSchedule(xmlBytes);

            // Assert creation time
            AssertDateTime(ParseDateTimeInUtc("2019-04-24T14:10:25Z"), testObject.CreationDateTime);

            // Assert schedule count
            Assert.AreEqual(1, testObject.ProductionSchedules.Count);

            // Assert request count
            var schedule1 = testObject.ProductionSchedules[0];
            Assert.AreEqual(2, schedule1.ProductionRequests.Count);


            // Assert request 1

            var request1 = schedule1.ProductionRequests[0];
            Assert.AreEqual(2, request1.SegmentRequirements.Count);

            // Asserting identifier
            Assert.AreEqual("my-identifier-1", request1.Identifier.Value);

            // Asserting hierarchy scope
            Assert.AreEqual("fsf", request1.HierarchyScopeObj.EquipmentIdentifier.Value);
            Assert.AreEqual(EquipmentElementLevelType.ProcessCell, request1.HierarchyScopeObj.EquipmentElementLevel);


            // Asserting one segment requirement

            var segReq1 = request1.SegmentRequirements[0];

            // Assert process segment identifier
            Assert.AreEqual("1", segReq1.ProcessSegmentIdentifier.Value);

            // Assert times
            AssertDateTime(ParseDateTimeInUtc("2019-04-24T15:00:00Z"), segReq1.EarliestStartTime.Value);
            AssertDateTime(ParseDateTimeInUtc("2019-04-24T15:30:00Z"), segReq1.LatestEndTime.Value);

            // Asserting equipment requirement
            Assert.AreEqual(1, segReq1.EquipmentRequirements.Count);
            EquipmentRequirement equipmentReq = segReq1.EquipmentRequirements[0];
            QuantityValue quantityEquipmentAvailability1 = equipmentReq.Quantities[0];
            QuantityValue quantityEquipmentAvailability2 = equipmentReq.Quantities[1];
            Assert.AreEqual("false", quantityEquipmentAvailability1.RawQuantityString);
            Assert.AreEqual("true", quantityEquipmentAvailability2.RawQuantityString);
            Assert.IsFalse(quantityEquipmentAvailability1.TryParseValueAsXmlBoolean());
            Assert.IsTrue(quantityEquipmentAvailability2.TryParseValueAsXmlBoolean());
            Assert.AreEqual(DataType.TypeType.booleanXml, quantityEquipmentAvailability1.DataType.Type);
            Assert.AreEqual(DataType.TypeType.booleanXml, quantityEquipmentAvailability2.DataType.Type);

            // Asserting material requirement
            Assert.AreEqual(2, segReq1.MaterialRequirements.Count);
            var matReq = segReq1.MaterialRequirements[0];
            // Asserting material definition ID
            Assert.AreEqual("matte", matReq.MaterialDefinitionIdentifiers[0].Value);
            // Asserting material lot ID
            Assert.AreEqual(1, matReq.MaterialLotIdentifiers.Count);
            Assert.AreEqual("psc2-15", matReq.MaterialLotIdentifiers[0].Value);
            // Assert material use
            Assert.AreEqual(MaterialUseType.Produced, matReq.MaterialUse.Value);
            // Assert quantity 1
            var matProdQuantity1 = matReq.Quantities[0];
            Assert.AreEqual("41.9", matProdQuantity1.RawQuantityString);
            Assert.AreEqual(41.9, matProdQuantity1.TryParseValueAsXmlDouble(), 0.001);
            Assert.AreEqual("t/h", matProdQuantity1.UnitOfMeasure);
            Assert.AreEqual(DataType.TypeType.doubleXml, matProdQuantity1.DataType.Type);
            Assert.AreEqual("ProdRate", matProdQuantity1.Key.Value);
            // Assert quantity 2
            var matProdQuantity2 = matReq.Quantities[1];
            Assert.AreEqual("11.9", matProdQuantity2.RawQuantityString);
            // Not asserting other fields, because this would be redundant to the previous quantity value.
            // -- Asserting assembly requirements. Only one field is included in the test,
            // because the assembly requirements have a structure similar to the enclosing requirements.
            Assert.AreEqual(2, matReq.AssemblyRequirements.Count);
            Assert.AreEqual("Cu", matReq.AssemblyRequirements[0].MaterialDefinitionIdentifiers[0].Value);
            Assert.AreEqual("S", matReq.AssemblyRequirements[1].MaterialDefinitionIdentifiers[0].Value);

            // Asserting another segment requirement (with nested segment requirement)

            var segReq2_1 = request1.SegmentRequirements[1].SegmentRequirements[0];
            AssertDateTime(ParseDateTimeInUtc("2019-04-24T15:31:00Z"), segReq2_1.EarliestStartTime.Value);


            // Assert request 2

            var request2 = schedule1.ProductionRequests[1];
            Assert.AreEqual("my-identifier-2", request2.Identifier.Value);
        }

        [TestMethod]
        public void ProcProdSched_ReadDateTimeKinds()
        {
            // Testing time values with various time zones or kinds.
            // The core functionality is tested in HelperTest, but now testing
            // the integration of XML documents.
            
            var filepath = TestCommon.TestHelper.TestFileFolder + "\\ProcessProductionSchedule_DateTimeKinds.xml";
            var xmlBytes = System.IO.File.ReadAllBytes(filepath);

            var testObject = new ProcessProductionSchedule(xmlBytes);
            var productionRequest = testObject.ProductionSchedules[0].ProductionRequests[0];

            var segmUnspec = productionRequest.SegmentRequirements[0];
            var segmUtc = productionRequest.SegmentRequirements[1];
            var segmPlus2 = productionRequest.SegmentRequirements[2];
            var segmMinus5 = productionRequest.SegmentRequirements[3];

            // 2019-04-24T15:00:00 in file
            AssertHourAndKind(13, DateTimeKind.Unspecified, segmUnspec.EarliestStartTime.Value);
            AssertHourAndKind(14, DateTimeKind.Unspecified, segmUnspec.LatestEndTime.Value);

            // 2019-04-24T15:00:00Z in file
            AssertHourAndKind(14, DateTimeKind.Utc, segmUtc.EarliestStartTime.Value);
            AssertHourAndKind(15, DateTimeKind.Utc, segmUtc.LatestEndTime.Value);

            // 2019-04-24T17:00:00+02:00 in file
            AssertHourAndKind(15, DateTimeKind.Utc, segmPlus2.EarliestStartTime.Value);
            AssertHourAndKind(16, DateTimeKind.Utc, segmPlus2.LatestEndTime.Value);

            // 2019-04-24T10:00:00-01:00 in file
            AssertHourAndKind(22, DateTimeKind.Utc, segmMinus5.EarliestStartTime.Value);
            AssertHourAndKind(23, DateTimeKind.Utc, segmMinus5.LatestEndTime.Value);
        }

        private void AssertHourAndKind(int expHour, DateTimeKind expKind, DateTime dt)
        {
            Assert.AreEqual(expHour, dt.Hour);
            Assert.AreEqual(expKind, dt.Kind);
        }
        
        [TestMethod]
        public void ProcProdSched_ReadEmptySched()
        {
            // The schedule is empty in this test
            
            var filepath = TestCommon.TestHelper.TestFileFolder + "\\ProcessProductionSchedule_EmptySched.xml";
            var xmlBytes = System.IO.File.ReadAllBytes(filepath);

            var testObject = new ProcessProductionSchedule(xmlBytes);

            AssertEmptyProcessMsg(testObject);
        }

        [TestMethod]
        public void ProcProdSched_ReadEmptyItems()
        {
            // There are empty items in the XML document in this test

            var filepath = TestCommon.TestHelper.TestFileFolder + "\\ProcessProductionSchedule_EmptyItems.xml";
            var xmlBytes = System.IO.File.ReadAllBytes(filepath);

            var testObject = new ProcessProductionSchedule(xmlBytes);

            // Asserting
            AssertEmptyItemsDoc(testObject);
        }

        [TestMethod]
        public void ProcProdSched_ReadInvalidDate()
        {
            // Testing reading a schedule with invalid values

            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Neg_ProcessProductionSchedule_InvalidDate.xml";
            var xmlBytes = System.IO.File.ReadAllBytes(filepath);

            try
            {
                new ProcessProductionSchedule(xmlBytes);
                Assert.Fail("Expected exception");
            }
            catch (XNeut.InvalidMessageException e)
            {
                Assert.IsTrue(e.Message.StartsWith("Failed to deserialise"));
            }
        }

        [TestMethod]
        public void ProcProdSched_ReadInvalidQuantityValues()
        {
            // Testing reading an invalid quantity value

            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Neg_ProcessProductionSchedule_InvalidQuantityValue.xml";
            var xmlBytes = System.IO.File.ReadAllBytes(filepath);

            var testObject = new ProcessProductionSchedule(xmlBytes);
            
            MaterialRequirement materialReq = testObject.ProductionSchedules[0].
                    ProductionRequests[0].SegmentRequirements[0].MaterialRequirements[0];

            QuantityValue quantityDouble = materialReq.Quantities[0];
            QuantityValue quantityBoolean = materialReq.Quantities[1];
            QuantityValue quantityInt = materialReq.Quantities[2];

            // To make sure the raw values have been received as expected:
            Assert.AreEqual("41fs.9", quantityDouble.RawQuantityString);
            Assert.AreEqual("faflse", quantityBoolean.RawQuantityString);
            Assert.AreEqual("0r3", quantityInt.RawQuantityString);

            // Double
            TestCommon.TestHelper.AssertInvalidOperationException(() =>
            {
                quantityDouble.TryParseValueAsXmlDouble();
            },
            "Failed to parse double");

            // Boolean
            TestCommon.TestHelper.AssertInvalidOperationException(() =>
            {
                quantityBoolean.TryParseValueAsXmlBoolean();
            },
            "Failed to parse boolean");

            // Int32 (int)
            TestCommon.TestHelper.AssertInvalidOperationException(() =>
            {
                quantityInt.TryParseValueAsXmlInt();
            },
            "Failed to parse int");

            // Int64 (long)
            TestCommon.TestHelper.AssertInvalidOperationException(() =>
            {
                // Parsing the same value as for Int32
                quantityInt.TryParseValueAsXmlLong();
            },
            "Failed to parse long");
        }

        [TestMethod]
        public void ProcProdSched_ReadInvalidQuantityDataType()
        {
            // Testing reading an invalid quantity datatype

            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Neg_ProcessProductionSchedule_InvalidQuantityDataType.xml";
            var xmlBytes = System.IO.File.ReadAllBytes(filepath);

            AssertInvalidMessageException(() =>
            {
                new ProcessProductionSchedule(xmlBytes);
            },
            "Failed to read ProductionRequest [Unknown ID]: Failed to parse datatype");
        }

        [TestMethod]
        public void ProcProdSched_ReadInvalidEqElemLevel()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Neg_ProcessProductionSchedule_InvalidEqElemLevel.xml";
            var xmlBytes = System.IO.File.ReadAllBytes(filepath);

            AssertInvalidMessageException(() =>
            {
                new ProcessProductionSchedule(xmlBytes);
            },
            "Failed to read ProductionRequest [Unknown ID]: Invalid equipment element level");
        }

        [TestMethod]
        public void ProcProdSched_ReadInvalidMatUse()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Neg_ProcessProductionSchedule_InvalidMatUse.xml";
            var xmlBytes = System.IO.File.ReadAllBytes(filepath);

            AssertInvalidMessageException(() =>
            {
                new ProcessProductionSchedule(xmlBytes);
            },
            "Failed to read ProductionRequest [Unknown ID]: Invalid material use value");
        }

        [TestMethod]
        public void ProcProdSched_ReadInvalidCreationTime()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Neg_ProcessProductionSchedule_InvalidCreationTime.xml";
            var xmlBytes = System.IO.File.ReadAllBytes(filepath);

            AssertInvalidMessageException(() =>
            {
                new ProcessProductionSchedule(xmlBytes);
            },
            "Failed to deserialise ProcessProductionSchedule from XML");
        }

        [TestMethod]
        public void ProcProdSched_ReadSegmentEndBeforeStart()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Neg_ProcessProductionSchedule_EndBeforeStart.xml";
            var xmlBytes = System.IO.File.ReadAllBytes(filepath);

            AssertInvalidMessageException(() =>
            {
                new ProcessProductionSchedule(xmlBytes);
            },
            "Failed to read ProductionRequest [Unknown ID]: Segment end must not be before start");
        }

        [TestMethod]
        public void ProcProdSched_ReadSchedulingParameters()
        {
            // Testing the reading of scheduling parameters

            var filepath = TestCommon.TestHelper.TestFileFolder + "\\ProcessProductionSchedule_SchedulingParams.xml";
            var xmlBytes = System.IO.File.ReadAllBytes(filepath);

            var testObject = new ProcessProductionSchedule(xmlBytes);
            
            // Get the parameters data record
            var productionReq = testObject.ProductionSchedules[0].ProductionRequests[0];
            var parameters = new CMeas.Item_DataRecord((SXml.XmlNode[])productionReq.SchedulingParameters);
            
            // Asserting just one parameter, because the Item_DataRecord class is
            // not a part of this application.
            var parameter = (CMeas.Item_Measurement)parameters["SomeParam1"];

            Assert.AreEqual(10.6, parameter.Value, 0.0001);
            Assert.AreEqual("t/h", parameter.UnitOfMeasure);
        }

        [TestMethod]
        public void ProcProdSched_Write()
        {
            // Serialising, validating and deserialising
            byte[] xmlData = CreateObjectForTestWrite().ToXmlBytes();
            Validate(xmlData);
            ProcessProductionSchedule testObject2 = new ProcessProductionSchedule(xmlData);

            // Assert creation time
            TestCommon.TestHelper.AssertDateTime(ParseDateTimeInUtc("2019-05-09T12:20:19Z"), testObject2.CreationDateTime);

            ProductionSchedule schedule = testObject2.ProductionSchedules[0];

            // Assert request count
            Assert.AreEqual(2, schedule.ProductionRequests.Count);

            // Asserting a production request
            ProductionRequest request1 = schedule.ProductionRequests[0];
            Assert.AreEqual(2, request1.SegmentRequirements.Count);

            // Asserting identifier
            Assert.AreEqual("some-id", request1.Identifier.Value);

            // Asserting a hierarchy scope
            Assert.AreEqual("psc3", request1.HierarchyScopeObj.EquipmentIdentifier.Value);
            Assert.AreEqual(EquipmentElementLevelType.ProcessCell, request1.HierarchyScopeObj.EquipmentElementLevel);

            // Asserting a segment requirement
            SegmentRequirement segReq = request1.SegmentRequirements[0];
            Assert.AreEqual("1", segReq.ProcessSegmentIdentifier.Value);
            AssertDateTime(ParseDateTimeInUtc("2019-05-09T13:36:02Z"), segReq.EarliestStartTime.Value);
            AssertDateTime(ParseDateTimeInUtc("2019-05-09T13:37:02Z"), segReq.LatestEndTime.Value);
            Assert.AreEqual(1, segReq.MaterialRequirements.Count);
            Assert.AreEqual(1, segReq.EquipmentRequirements.Count);

            // Asserting nested segment requirement
            SegmentRequirement segReqNested = segReq.SegmentRequirements[0];
            AssertDateTime(ParseDateTimeInUtc("2019-08-29T15:31:38Z"), segReqNested.EarliestStartTime.Value);

            // Asserting equipment requirement
            EquipmentRequirement eqReq = segReq.EquipmentRequirements[0];
            Assert.AreEqual(1, eqReq.Quantities.Count);
            Assert.IsTrue(eqReq.Quantities[0].TryParseValueAsXmlBoolean());

            MaterialRequirement matReq = segReq.MaterialRequirements[0];

            // Asserting material definition ID
            Assert.AreEqual(1, matReq.MaterialDefinitionIdentifiers.Count);
            Assert.AreEqual("slag", matReq.MaterialDefinitionIdentifiers[0].Value);

            // Asserting a material lot ID
            Assert.AreEqual(1, matReq.MaterialLotIdentifiers.Count);
            Assert.AreEqual("my-lot-1", matReq.MaterialLotIdentifiers[0].Value);

            // Asserting material use
            Assert.AreEqual(MaterialUseType.Produced, matReq.MaterialUse.Value);

            // Asserting a material quantity
            Assert.AreEqual(1, matReq.Quantities.Count);
            QuantityValue quantity = matReq.Quantities[0];
            Assert.AreEqual("12.2", quantity.RawQuantityString);
            Assert.AreEqual(12.2, quantity.TryParseValueAsXmlDouble(), 0.001);
            Assert.AreEqual("t", quantity.UnitOfMeasure);
            Assert.AreEqual(DataType.TypeType.doubleXml, quantity.DataType.Type);
            Assert.AreEqual("my-mat-key", quantity.Key.Value);

            // Asserting an assembly requirement
            Assert.AreEqual(1, matReq.AssemblyRequirements.Count);
            Assert.AreEqual("Ni", matReq.AssemblyRequirements[0].MaterialDefinitionIdentifiers[0].Value);
        }

        private ProcessProductionSchedule CreateObjectForTestWrite()
        {
            // Applying identifiers (such as "SEG1") to items to enable a
            // verification that this test implementation has same items
            // as those in other environments, particularly Java.
            
            // SEG1a Creating a segment requirement
            SegmentRequirement segReq1 = new SegmentRequirement
            {
                ProcessSegmentIdentifier = new IdentifierType("1"),
                EarliestStartTime = ParseDateTimeInUtc("2019-05-09T13:36:02Z"),
                LatestEndTime = ParseDateTimeInUtc("2019-05-09T13:37:02Z")
            };

            // EQ1 Add equipment requirement
            EquipmentRequirement eqReq = new EquipmentRequirement()
            {
                Quantities = new List<QuantityValue>()
                {
                    new QuantityValue(true)
                }
            };
            segReq1.EquipmentRequirements.Add(eqReq);

            // MAT1 Add material requirement
            MaterialRequirement matReq = new MaterialRequirement()
            {
                MaterialDefinitionIdentifiers = new List<IdentifierType>()
                {
                    new IdentifierType("slag")
                },

                MaterialLotIdentifiers = new List<IdentifierType>()
                {
                    new IdentifierType("my-lot-1")
                },

                MaterialUse = new MaterialUse(MaterialUseType.Produced),

                Quantities = new List<QuantityValue>()
                {
                    new QuantityValue(12.2)
                    {
                        UnitOfMeasure = "t",
                        Key = new IdentifierType("my-mat-key")
                    }
                },

                AssemblyRequirements = new List<MaterialRequirement>()
                {
                    new MaterialRequirement()
                    {
                        MaterialDefinitionIdentifiers = new List<IdentifierType>()
                        {
                            new IdentifierType("Ni")
                        }
                    }
                }
            };
            segReq1.MaterialRequirements.Add(matReq);

            // SEG1-1 Add nested segment requirement
            segReq1.SegmentRequirements.Add(new SegmentRequirement()
            {
                EarliestStartTime = ParseDateTimeInUtc("2019-08-29T15:31:38Z")
            });
            
            // PROD1a Create one production request (for the unit "psc3")
            ProductionRequest request1 = new ProductionRequest
            {
                // PROD1-ID Set identifier
                Identifier = new IdentifierType("some-id"),
                
                // PROD1-HS Set hierarchy scope
                HierarchyScopeObj = new HierarchyScope(
                    new IdentifierType("psc3"),
                    EquipmentElementLevelType.ProcessCell
                    ),

                SegmentRequirements = new List<SegmentRequirement>()
                {
                    // SEG1b Add segment requirement to production request
                    segReq1,
                    
                    // SEG2 Add another (empty) segment requirement
                    new SegmentRequirement()
                }
            };

            // SCH Create schedule object
            ProductionSchedule schedule = new ProductionSchedule()
            {
                ProductionRequests = new List<ProductionRequest>()
                {
                    // PROD1b Add the production request to schedule
                    request1,

                    // PROD2 Adding another (empty) production request
                    new ProductionRequest()
                }
            };

            // PROPS Creating object to be serialised
            ProcessProductionSchedule testObject1 = new ProcessProductionSchedule()
            {
                // PROPS-CR Set creation time
                CreationDateTime = ParseDateTimeInUtc("2019-05-09T12:20:19Z")
            };

            // Add schedule to the test object
            testObject1.ProductionSchedules.Add(schedule);
            return testObject1;
        }

        [TestMethod]
        public void ProcProdSched_WriteEmptySchedule()
        {
            // Testing if write works when the schedule is empty.

            // Creating an object to be serialised
            var schedule = new ProductionSchedule();
            var testObject1 = new ProcessProductionSchedule();
            var approximateCreationTime = DateTime.Now.ToUniversalTime();
            testObject1.ProductionSchedules.Add(schedule);

            // Serialising validating and deserialising. The test will likely fails here if it fails.
            var xmlData = testObject1.ToXmlBytes();
            Validate(xmlData);
            var testObjectIn = new ProcessProductionSchedule(xmlData);

            // Asserting
            AssertEmptyProcessMsg(testObjectIn);

            // Asserting creation time. Expecting it to be the creation time of the object.
            // !!! This assertion can fail if this test case is run first because there is
            // a long delay when the XML libraries are initialised. !!!
            long difference_ms = approximateCreationTime.Ticks - testObjectIn.CreationDateTime.Ticks;
            Assert.IsTrue(Math.Abs(difference_ms) < 500);
        }

        [TestMethod]
        public void ProcProdSched_WriteEmptyItems()
        {
            // Testing if write works when there are empty items in the schedule

            // Create this production request here, because otherwise
            // there would be too many nested blocks to be readable
            var productionRequest3 = new ProductionRequest()
            {
                SegmentRequirements = new List<SegmentRequirement>()
                {
                    new SegmentRequirement()
                    {
                        EquipmentRequirements = new List<EquipmentRequirement>()
                        {
                            new EquipmentRequirement()
                            {
                                // 3) Empty equipment requirement
                            }
                        },

                        MaterialRequirements = new List<MaterialRequirement>()
                        {
                            new MaterialRequirement()
                            {
                                // 4) Empty material requirement
                            },
                            new MaterialRequirement()
                            {
                                Quantities = new List<QuantityValue>()
                                {
                                    // 5) Empty quantity value
                                    new QuantityValue((string)null)
                                }
                            }
                        }
                    }
                }
            };

            var testObject1 = new ProcessProductionSchedule()
            {
                ProductionSchedules = new List<ProductionSchedule>()
                {
                    new ProductionSchedule()
                    {
                        ProductionRequests = new List<ProductionRequest>()
                        {
                            // 1) Empty production request
                            new ProductionRequest(),

                            new ProductionRequest()
                            {
                                HierarchyScopeObj = new HierarchyScope(
                                    new IdentifierType("psc2"),
                                    EquipmentElementLevelType.ProcessCell
                                    ),

                                SegmentRequirements = new List<SegmentRequirement>()
                                {
                                    // 2) Empty segment requirement
                                    new SegmentRequirement()
                                }
                            },

                            productionRequest3
                        }
                    }
                }
            };
            
            // Serialising, validating and deserialising. The test will likely fails here if it fails.
            var xmlData = testObject1.ToXmlBytes();
            Validate(xmlData);
            var testObjectIn = new ProcessProductionSchedule(xmlData);

            // Asserting
            AssertEmptyItemsDoc(testObjectIn);
        }
        
        [TestMethod]
        public void ProcProdSched_WriteSchedulingParameters()
        {
            // Testing the writing of scheduling parameters

            // Creating a schedule to be serialised
            var schedule = new ProductionSchedule();
            var testObject1 = new ProcessProductionSchedule();
            testObject1.ProductionSchedules.Add(schedule);
            
            var schedulingParameters = new CMeas.Item_DataRecord()
            {
                { "myparam", new CMeas.Item_Count(3) }
            };

            // Creating a production request with scheduling parameters
            var productionRequest = new ProductionRequest
            {
                // Setting scheduling parameters. Not testing the parameters thorougly
                // because the data record is tested elsewhere.
                SchedulingParameters = schedulingParameters.ToDataRecordPropertyProxy()
            };
            
            schedule.ProductionRequests.Add(productionRequest);

            // Serialising validating and deserialising. The test will likely fails here if it fails.
            var xmlData = testObject1.ToXmlBytes();
            var xmlString = System.Text.Encoding.UTF8.GetString(xmlData); // Just for debugging
            Validate(xmlData);
            
            // Asserting
            AssertSchedulingParameters(new ProcessProductionSchedule(xmlData));
        }

        [TestMethod]
        public void ProcProdSched_WriteNonUtcTimes()
        {
            // Expecting an error when there is an attempt to assign a non-UTC timestamp
            
            var sched = new ProcessProductionSchedule();
            var segm = new SegmentRequirement();

            AssertDateTimeException(() =>
            {
                sched.CreationDateTime = DateTime.Parse("2020-02-20T13:09:00"); // expecting local kind by default
            }, "DateTime kind must be UTC");

            AssertDateTimeException(() =>
            {
                segm.EarliestStartTime = DateTime.Parse("2020-02-20T13:09:00"); // expecting local kind by default
            }, "DateTime kind must be UTC");

            AssertDateTimeException(() =>
            {
                segm.LatestEndTime = DateTime.Parse("2020-02-20T13:09:00"); // expecting local kind by default
            }, "DateTime kind must be UTC");
        }

        [TestMethod]
        public void ProcProdSched_WriteSegmentStartAfterEnd()
        {
            // Segment start must not be after end

            var startTime = ParseDateTimeInUtc("2020-02-20T14:37:00Z");
            var faultySegment = new SegmentRequirement()
            {
                EarliestStartTime = startTime,
                LatestEndTime = startTime.AddSeconds(-1) // Before start
            };

            var processProdSched = new ProcessProductionSchedule()
            {
                ProductionSchedules = new List<ProductionSchedule>
                {
                    new ProductionSchedule()
                    {
                        ProductionRequests = new List<ProductionRequest>()
                        {
                            new ProductionRequest()
                            {
                                SegmentRequirements = new List<SegmentRequirement>
                                {
                                    faultySegment
                                }
                            }
                        }
                    }
                }
            };

            // Expecting an error as message serialisation is requested
            AssertDateTimeException(() =>
            {
                processProdSched.ToXmlBytes();
            }, "Start of segment must not be after end");
        }

        private void AssertSchedulingParameters(ProcessProductionSchedule testObjectIn)
        {
            // Use a separate function for assert to prevent asserting the wrong object.
            
            // Asserting parameters. Just a simple assert, because Item_DataRecord
            // is tested elsewhere.
            var paramNodes = (SXml.XmlNode[])testObjectIn.ProductionSchedules[0].ProductionRequests[0].SchedulingParameters;
            var paramsRecord = new CMeas.Item_DataRecord(paramNodes);
            Assert.AreEqual(1, paramsRecord.ItemNames.Count);
            Assert.AreEqual(3, ((CMeas.Item_Count)paramsRecord["myparam"]).Value);
        }
        
        private void AssertEmptyItemsDoc(ProcessProductionSchedule testObjectIn)
        {
            // This function asserts the object in "ProcessProductionSchedule_EmptyItems.xml".
            // The same function is also used as writing is tested.

            // One schedule expected
            Assert.AreEqual(1, testObjectIn.ProductionSchedules.Count);
            var schedule1 = testObjectIn.ProductionSchedules[0];

            // Three production requests expected
            Assert.AreEqual(3, schedule1.ProductionRequests.Count);
            var request1 = schedule1.ProductionRequests[0];
            var request2 = schedule1.ProductionRequests[1];
            var request3 = schedule1.ProductionRequests[2];

            // 1) Assert empty production request (request 1)
            Assert.IsNull(request1.Identifier);
            Assert.IsNull(request1.HierarchyScopeObj);
            Assert.AreEqual(0, request1.SegmentRequirements.Count);

            // 2) Asserting segment requirement (request 2)
            Assert.AreEqual(1, request2.SegmentRequirements.Count);
            AssertEmptySegmentRequirement(request2.SegmentRequirements[0]);

            // 3) Asserting empty equipment requirement (request 3)
            var eqReq_3_1 = request3.SegmentRequirements[0].EquipmentRequirements[0];
            Assert.AreEqual(0, eqReq_3_1.Quantities.Count);

            // 4) Asserting minimal material requirement (request 3)
            var matReq_3_1 = request3.SegmentRequirements[0].MaterialRequirements[0];
            Assert.AreEqual(0, matReq_3_1.MaterialDefinitionIdentifiers.Count);
            Assert.AreEqual(0, matReq_3_1.MaterialLotIdentifiers.Count);
            Assert.IsNull(matReq_3_1.MaterialUse);
            Assert.AreEqual(0, matReq_3_1.Quantities.Count);
            Assert.AreEqual(0, matReq_3_1.AssemblyRequirements.Count);

            // 5) Asserting minimal quantity value (request 3)
            var matReq_3_2 = request3.SegmentRequirements[0].MaterialRequirements[1];
            var quantityX = matReq_3_2.Quantities[0];
            Assert.IsNull(quantityX.DataType);
            Assert.IsTrue(string.IsNullOrEmpty(quantityX.UnitOfMeasure));
            Assert.IsNull(quantityX.Key);
        }

        private void AssertEmptySegmentRequirement(SegmentRequirement segReq)
        {
            // Process segment ID
            Assert.IsNull(segReq.ProcessSegmentIdentifier);

            // Times
            Assert.IsFalse(segReq.EarliestStartTime.HasValue);
            Assert.IsFalse(segReq.LatestEndTime.HasValue);

            // Equipment
            Assert.AreEqual(0, segReq.EquipmentRequirements.Count);

            // Material
            Assert.AreEqual(0, segReq.MaterialRequirements.Count);
            
            // Segment (nested)
            Assert.AreEqual(0, segReq.SegmentRequirements.Count);
        }

        private void AssertEmptyProcessMsg(ProcessProductionSchedule testObject)
        {
            Assert.AreEqual(1, testObject.ProductionSchedules.Count);
            Assert.AreEqual(0, testObject.ProductionSchedules[0].ProductionRequests.Count);
        }
        
        private void AssertDateTime(DateTime expected, DateTime actual)
        {
            TestCommon.TestHelper.AssertDateTime(expected, actual);
        }

        private void AssertInvalidMessageException(Action testWorker, string expectedMessageStart)
        {
            // Expecting exception
            try
            {
                testWorker.Invoke();
                Assert.Fail("InvalidMessageException was expected");
            }
            catch (XNeut.InvalidMessageException e)
            {
                Assert.IsTrue(e.Message.StartsWith(expectedMessageStart), "Unexpected exception message " + e.Message);
            }
        }

        private void Validate(byte[] xmlBytes)
        {
            // Validating the document
            var validator = new TestCommon.Validator(TestCommon.Validator.SchemaType.B2mml | TestCommon.Validator.SchemaType.Swe);

            using (var xmlStream = new System.IO.MemoryStream(xmlBytes))
            {
                validator.Validate(xmlStream, typeof(XsdNs.ProcessProductionScheduleType));
            }
        }

        private void AssertDateTimeException(Action action, string expectedMsgStart)
        {
            try
            {
                action();
                Assert.Fail("Expected an exception");
            }
            catch (XNeut.DateTimeException e)
            {
                Assert.IsTrue(e.Message.StartsWith(expectedMsgStart), "Unexpected error message " + e.Message);
            }
        }

        private DateTime ParseDateTimeInUtc(string s)
        {
            return TestCommon.TestHelper.ParseDateTimeInUtc(s);
        }
    }
}
