// 
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
// Author: Petri Kannisto, Tampere University, Finland
// Last modified: 4/2020
// 
// This file has been derived the XML schemata of Business to Manufacturing 
// Markup Language (B2MML). B2MML has the following license agreement:
// 
// 'This Manufacturing Enterprise Solutions Association (MESA International)
// Work (including specifications, documents, software, and related items)
// referred to as the Business To Manufacturing Markup Language (B2MML) is
// provided by the copyright holders under the following license. Permission
// to use, copy, modify, or redistribute this Work and its documentation, with
// or without modification, for any purpose and without fee or royalty is
// hereby granted provided MESA is acknowledged as the originator of this Work
// using the following statement: "The Business To Manufacturing Markup
// Language (B2MML) is used courtesy of the Manufacturing Enterprise Solutions
// Association (MESA International)." In no event shall MESA, its members, or
// any third party be liable for any costs, expenses, losses, damages or
// injuries incurred by use of the Work or as a result of this agreement.'
// 
// The B2MML XML schemata are available at http://www.mesa.org/en/B2MML.asp
// 
// Please note: This software has *not* received any official compliance check
// with B2MML. This software was *not* created by MESA International.

using System;
using System.Collections.Generic;
using XsdNs = Cocop.MessageSerialiser.Biz.XsdGen;
using XNeut = Cocop.MessageSerialiser.Biz.Neutral;

namespace Cocop.MessageSerialiser.Biz
{
    /// <summary>
    /// Represents a production segment.
    /// </summary>
    public sealed class SegmentRequirement
    {
        private DateTime? m_earliestStartTime = null;
        private DateTime? m_latestEndTime = null;


        /// <summary>
        /// Constructor.
        /// </summary>
        public SegmentRequirement()
        {
            EquipmentRequirements = new List<EquipmentRequirement>();
            MaterialRequirements = new List<MaterialRequirement>();
            SegmentRequirements = new List<SegmentRequirement>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="proxy">XML proxy.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        internal SegmentRequirement(XsdNs.SegmentRequirementType proxy)
            : this() // Call the default constructor
        {
            try
            {
                if (proxy.ProcessSegmentID != null)
                {
                    ProcessSegmentIdentifier = new IdentifierType(proxy.ProcessSegmentID);
                }

                m_earliestStartTime = TryGetTime(proxy.EarliestStartTime);
                m_latestEndTime = TryGetTime(proxy.LatestEndTime);

                // Check if end is before start
                if (m_earliestStartTime.HasValue && m_latestEndTime.HasValue &&
                    m_earliestStartTime.Value > m_latestEndTime.Value)
                {
                    var msg = string.Format("Segment end must not be before start; start at {0} UTC",
                        m_earliestStartTime.Value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    throw new XNeut.InvalidMessageException(msg);
                }

                if (proxy.EquipmentRequirement != null)
                {
                    foreach (var req in proxy.EquipmentRequirement)
                    {
                        EquipmentRequirements.Add(new EquipmentRequirement(req)); // throws InvalidMessageException
                    }
                }

                if (proxy.MaterialRequirement != null)
                {
                    foreach (var req in proxy.MaterialRequirement)
                    {
                        MaterialRequirements.Add(new MaterialRequirement(req)); // throws InvalidMessageException
                    }
                }

                if (proxy.SegmentRequirement != null)
                {
                    foreach (var req in proxy.SegmentRequirement)
                    {
                        SegmentRequirements.Add(new SegmentRequirement(req)); // throws InvalidMessageException
                    }
                }
            }
            catch (NullReferenceException e)
            {
                throw new XNeut.InvalidMessageException("Failed to read SegmentRequirement - something required is missing", e);
            }
        }

        private DateTime? TryGetTime(XsdNs.DateTimeType dtRaw)
        {
            return dtRaw == null ? (DateTime?)null : XNeut.Helper.DateTimeToUtcIfPossible(dtRaw.Value);
        }
        
        /// <summary>
        /// Related process segment ID.
        /// </summary>
        public IdentifierType ProcessSegmentIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// The earliest start time.
        /// </summary>
        /// <exception cref="XNeut.DateTimeException">Thrown if there is an attempt to set a non-UTC value.</exception>
        public DateTime? EarliestStartTime
        {
            get
            {
                return m_earliestStartTime;
            }
            set
            {
                if (value.HasValue)
                {
                    XNeut.Helper.ExpectDateTimeIsUtc(value.Value); // throws DateTimeException
                }

                m_earliestStartTime = value;
            }
        }

        /// <summary>
        /// The latest end time.
        /// </summary>
        /// <exception cref="XNeut.DateTimeException">Thrown if there is an attempt to set a non-UTC value.</exception>
        public DateTime? LatestEndTime
        {
            get
            {
                return m_latestEndTime;
            }
            set
            {
                if (value.HasValue)
                {
                    XNeut.Helper.ExpectDateTimeIsUtc(value.Value); // throws DateTimeException
                }

                m_latestEndTime = value;
            }
        }

        /// <summary>
        /// Equipment requirements.
        /// </summary>
        public List<EquipmentRequirement> EquipmentRequirements
        {
            get;
            set;
        }

        /// <summary>
        /// Material requirements.
        /// </summary>
        public List<MaterialRequirement> MaterialRequirements
        {
            get;
            set;
        }

        /// <summary>
        /// (Nested) segment requirements.
        /// </summary>
        public List<SegmentRequirement> SegmentRequirements
        {
            get;
            set;
        }

        /// <summary>
        /// Generates an XML proxy from the object.
        /// </summary>
        /// <returns>XML proxy.</returns>
        /// <exception cref="XNeut.DateTimeException">Thrown if there is a segment that has EarliestStartTime
        /// after LatestEndTime.</exception>
        internal XsdNs.SegmentRequirementType ToXmlProxy()
        {
            XsdNs.ProcessSegmentIDType segmentIdProxy = null;

            // Process segment ID specified?
            if (ProcessSegmentIdentifier != null)
            {
                // Creating segment ID proxy
                segmentIdProxy = new XsdNs.ProcessSegmentIDType();
                ProcessSegmentIdentifier.PopulateXmlProxy(segmentIdProxy);
            }

            var retval = new XsdNs.SegmentRequirementType()
            {
                ProcessSegmentID = segmentIdProxy,

                EquipmentRequirement = BuildEquipmentReqsForProxy(),
                MaterialRequirement = BuildMaterialReqsForProxy(),
                SegmentRequirement = BuildSegmentReqsForProxy()
            };

            // Making sure start is not after end
            if (m_earliestStartTime.HasValue && m_latestEndTime.HasValue &&
                m_earliestStartTime > m_latestEndTime)
            {
                var msg = string.Format("Start of segment must not be after end (starting at {0} UTC)",
                    m_earliestStartTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                throw new XNeut.DateTimeException(msg);
            }

            // Add time values
            if (m_earliestStartTime.HasValue)
            {
                retval.EarliestStartTime = new XsdNs.EarliestStartTimeType()
                {
                    Value = XNeut.Helper.DateTimeForSerialisation(m_earliestStartTime.Value)
                };
            }
            if (m_latestEndTime.HasValue)
            {
                retval.LatestEndTime = new XsdNs.LatestEndTimeType()
                {
                    Value = XNeut.Helper.DateTimeForSerialisation(m_latestEndTime.Value)
                };
            }

            return retval;
        }

        private XsdNs.EquipmentRequirementType[] BuildEquipmentReqsForProxy()
        {
            var retval = new XsdNs.EquipmentRequirementType[EquipmentRequirements.Count];

            for (int a = 0; a < EquipmentRequirements.Count; ++a)
            {
                retval[a] = EquipmentRequirements[a].ToXmlProxy();
            }

            return retval;
        }

        private XsdNs.MaterialRequirementType[] BuildMaterialReqsForProxy()
        {
            var retval = new XsdNs.MaterialRequirementType[MaterialRequirements.Count];

            for (int a = 0; a < MaterialRequirements.Count; ++a)
            {
                retval[a] = MaterialRequirements[a].ToXmlProxy();
            }
            
            return retval;
        }

        private XsdNs.SegmentRequirementType[] BuildSegmentReqsForProxy()
        {
            var retval = new XsdNs.SegmentRequirementType[SegmentRequirements.Count];

            for (int a = 0; a < SegmentRequirements.Count; ++a)
            {
                retval[a] = SegmentRequirements[a].ToXmlProxy();
            }

            return retval;
        }
    }
}
