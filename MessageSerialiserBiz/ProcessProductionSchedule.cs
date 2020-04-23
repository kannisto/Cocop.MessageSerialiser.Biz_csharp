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
    /// Represents an instruction to apply a production schedule.
    /// </summary>
    public sealed class ProcessProductionSchedule
    {
        private DateTime m_creationDateTime = DateTime.Now.ToUniversalTime();


        /// <summary>
        /// Constructor.
        /// </summary>
        public ProcessProductionSchedule()
        {
            m_creationDateTime = DateTime.Now.ToUniversalTime();
            ProductionSchedules = new List<ProductionSchedule>();
        }

        /// <summary>
        /// Constructor. Use this to deserialise from XML.
        /// </summary>
        /// <param name="xmlBytes">XML data.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        public ProcessProductionSchedule(byte[] xmlBytes)
            : this()
        {
            XsdNs.ProcessProductionScheduleType proxy = null;

            // Generating proxy
            try
            {
                proxy = (XsdNs.ProcessProductionScheduleType)XNeut.Helper.DeserialiseFromXml(typeof(XsdNs.ProcessProductionScheduleType), xmlBytes);

                // Processing the proxy
                ReadFieldValuesFromXmlProxy(proxy); // throws InvalidMessageException
            }
            catch (InvalidOperationException e)
            {
                throw new XNeut.InvalidMessageException("Failed to deserialise ProcessProductionSchedule from XML", e);
            }
        }

        private void ReadFieldValuesFromXmlProxy(XsdNs.ProcessProductionScheduleType proxy)
        {
            try
            {
                // Read creation time
                m_creationDateTime = XNeut.Helper.DateTimeToUtcIfPossible(proxy.ApplicationArea.CreationDateTime.Value);
                
                // The schema requires at least one schedule. Therefore, if
                // ProductionSchedule[] is null, the document is invalid -> no null check here.
                foreach (var scheduleProxy in proxy.DataArea.ProductionSchedule)
                {
                    ProductionSchedules.Add(new ProductionSchedule(scheduleProxy));
                }
            }
            catch (NullReferenceException e)
            {
                throw new XNeut.InvalidMessageException("Failed to read ProcessProductionSchedule - something required is missing", e);
            }
        }

        /// <summary>
        /// Enclosed production schedules.
        /// </summary>
        public List<ProductionSchedule> ProductionSchedules
        {
            get;
            set;
        }

        /// <summary>
        /// Creation time.
        /// </summary>
        /// <exception cref="XNeut.DateTimeException">Thrown if there is an attempt to set a non-UTC value.</exception>
        public DateTime CreationDateTime
        {
            get
            {
                return m_creationDateTime;
            }
            set
            {
                XNeut.Helper.ExpectDateTimeIsUtc(value); // throws DateTimeException
                m_creationDateTime = value;
            }
        }

        /// <summary>
        /// Serialises the object to XML.
        /// </summary>
        /// <returns>XML data.</returns>
        public byte[] ToXmlBytes()
        {
            var proxy = new XsdNs.ProcessProductionScheduleType
            {
                // Set release ID (obligatory in the XML schema)
                releaseID = "1",

                // Create application area
                ApplicationArea = new XsdNs.TransApplicationAreaType()
                {
                    // Set creation datetime
                    CreationDateTime = new XsdNs.DateTimeType
                    {
                        Value = XNeut.Helper.DateTimeForSerialisation(m_creationDateTime)
                    }
                },

                // Creating data area
                DataArea = new XsdNs.ProcessProductionScheduleTypeDataArea()
                {
                    Process = new XsdNs.TransProcessType(),

                    // Adding schedules
                    ProductionSchedule = CreateScheduleProxies(out ICollection<Type> extraTypes)
                }
            };

            // Serialising to XML
            return XNeut.Helper.ToXmlBytes(obj: proxy, extraTypes: extraTypes);
        }

        private XsdNs.ProductionScheduleType[] CreateScheduleProxies(out ICollection<Type> extraTypes)
        {
            var retval = new XsdNs.ProductionScheduleType[ProductionSchedules.Count];
            var extraTypesSet = new HashSet<Type>();

            for (int a = 0; a < ProductionSchedules.Count; ++a)
            {
                retval[a] = ProductionSchedules[a].ToXmlProxy(out ICollection<Type> extraTypesTemp);

                // Adding any required extra types
                extraTypesSet.UnionWith(extraTypesTemp);
            }

            extraTypes = extraTypesSet;
            return retval;
        }
    }
}
