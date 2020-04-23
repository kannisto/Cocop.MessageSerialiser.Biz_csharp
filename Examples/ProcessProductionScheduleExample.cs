//
// Please make sure to read and understand the files README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 2/2020
// Last modified: 4/2020

using System;
using SysColl = System.Collections.Generic;
using MsgBiz = Cocop.MessageSerialiser.Biz;

namespace Examples
{
    class ProcessProductionScheduleExample
    {
        // This example shows how to create and read a ProcessProductionSchedule
        // message in XML (based on the XML schemata of B2MML).
        //
        // Please note that this example does not use all fields available in the API.
        // On the other hand, the API only implements are fraction of B2MML.
        // "ProcessProductionSchedule" is only one of the message types of B2MML.
        // Please see the ANSI/ISA-95 standard or B2MML documentation to learn more
        // about message types and messaging patterns.

        public byte[] CreateXml()
        {
            // This function procudes a schedule for an imaginary batch process,
            // where materials are cooked in a "cooker".
            // There are 3 segments: "charge", "cook" and "discharge".
            // Charge takes raw material in, whereas discharge takes the end product out.

            var processStart = DateTime.Now.AddMinutes(40).ToUniversalTime();

            // Creating segment requirements. The segments are "charge", "cook" and "discharge".
            var segmentCharge = new MsgBiz.SegmentRequirement
            {
                ProcessSegmentIdentifier = new MsgBiz.IdentifierType("charge"),
                EarliestStartTime = processStart,
                LatestEndTime = processStart.AddMinutes(15)
            };
            var segmentCook = new MsgBiz.SegmentRequirement
            {
                ProcessSegmentIdentifier = new MsgBiz.IdentifierType("cook"),
                EarliestStartTime = processStart.AddMinutes(15),
                LatestEndTime = processStart.AddMinutes(75)
            };
            var segmentDischarge = new MsgBiz.SegmentRequirement
            {
                ProcessSegmentIdentifier = new MsgBiz.IdentifierType("discharge"),
                EarliestStartTime = processStart.AddMinutes(75),
                LatestEndTime = processStart.AddMinutes(90)
            };

            // Specifying the required equipment
            var equipmentReq = new MsgBiz.EquipmentRequirement()
            {
                Quantities = new SysColl.List<MsgBiz.QuantityValue>()
                {
                    // One cooker is required for the process
                    new MsgBiz.QuantityValue(1)
                    {
                        Key = new MsgBiz.IdentifierType("cooker")
                    }
                }
            };
            segmentCharge.EquipmentRequirements.Add(equipmentReq);
            segmentCook.EquipmentRequirements.Add(equipmentReq);
            segmentDischarge.EquipmentRequirements.Add(equipmentReq);

            // Specifying the required raw materials
            var materialWater = CreateMaterialReq(
                use: MsgBiz.MaterialUseType.Consumed,
                id: "water", // This could be more specific
                quantity: 3.2,
                uom: "m3");
            var materialConcentrate = CreateMaterialReq(
                use: MsgBiz.MaterialUseType.Consumed,
                id: "concentrate-A2",
                quantity: 220,
                uom: "kg");
            segmentCharge.MaterialRequirements.Add(materialWater);
            segmentCharge.MaterialRequirements.Add(materialConcentrate);

            // Specifying the expected material output
            var materialEndProduct = CreateMaterialReq(
                use: MsgBiz.MaterialUseType.Produced,
                id: "product-A2",
                quantity: 3.1,
                uom: "t");
            // Specify a lot ID for the output
            materialEndProduct.MaterialLotIdentifiers.Add(
                new MsgBiz.IdentifierType("lot-A2-3101")
                );
            segmentDischarge.MaterialRequirements.Add(materialEndProduct);
            
            // Creating a production request. The schedule could contain multiple of these.
            // You could add multiple requests, one for each production unit, for instance.
            var productionRequest = new MsgBiz.ProductionRequest
            {
                // Set hierarchy scope
                HierarchyScopeObj = new MsgBiz.HierarchyScope(
                    new MsgBiz.IdentifierType("cooker-3"),
                    MsgBiz.EquipmentElementLevelType.ProcessCell
                    ),

                // Set segments
                SegmentRequirements = new SysColl.List<MsgBiz.SegmentRequirement>()
                {
                    segmentCharge, segmentCook, segmentDischarge
                }
            };

            // Creating a schedule object
            var schedule = new MsgBiz.ProductionSchedule()
            {
                ProductionRequests = new SysColl.List<MsgBiz.ProductionRequest>()
                { productionRequest }
            };
            
            var processScheduleRequest = new MsgBiz.ProcessProductionSchedule()
            {
                CreationDateTime = DateTime.Now.ToUniversalTime(),
                ProductionSchedules = new SysColl.List<MsgBiz.ProductionSchedule>()
                { schedule }
            };
            
            // Serialising to XML
            return processScheduleRequest.ToXmlBytes();
        }

        private MsgBiz.MaterialRequirement CreateMaterialReq(MsgBiz.MaterialUseType use, string id, double quantity, string uom)
        {
            // This specifies the use of material certain, e.g., consume or produce.
            return new MsgBiz.MaterialRequirement()
            {
                MaterialUse = new MsgBiz.MaterialUse(use),

                MaterialDefinitionIdentifiers = new SysColl.List<MsgBiz.IdentifierType>()
                { new MsgBiz.IdentifierType(id) },

                Quantities = new SysColl.List<MsgBiz.QuantityValue>()
                {
                    new MsgBiz.QuantityValue(quantity)
                    { UnitOfMeasure = uom }
                }
            };
        }

        public void ReadXml(byte[] xmlBytes)
        {
            MsgBiz.ProcessProductionSchedule processRequest;

            // Reading the message
            try
            {
                processRequest = new MsgBiz.ProcessProductionSchedule(xmlBytes);
            }
            catch (MsgBiz.Neutral.InvalidMessageException e)
            {
                throw new InvalidOperationException("Failed to read message: " + e.Message);
            }

            // Now, read message content and realise the schedule if possible
            //processRequest.ProductionSchedules[0].ProductionRequests[0].SegmentRequirements ...
        }
    }
}
