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
using SXml = System.Xml;
using XsdNs = Cocop.MessageSerialiser.Biz.XsdGen;
using XNeut = Cocop.MessageSerialiser.Biz.Neutral;

namespace Cocop.MessageSerialiser.Biz
{
    /// <summary>
    /// Represents a request for a certain production entity.
    /// </summary>
    public sealed class ProductionRequest
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ProductionRequest()
        {
            SegmentRequirements = new List<SegmentRequirement>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="proxy">XML proxy.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        internal ProductionRequest(XsdNs.ProductionRequestType proxy)
        {
            SegmentRequirements = new List<SegmentRequirement>();

            try
            {
                // Read identifier
                if (proxy.ID != null)
                {
                    Identifier = new IdentifierType(proxy.ID);
                }
                
                // Read hierarchy scope
                if (proxy.HierarchyScope != null)
                {
                    HierarchyScopeObj = new HierarchyScope(proxy.HierarchyScope); // throws InvalidMessageException
                }

                // Read segment requirements
                if (proxy.SegmentRequirement != null)
                {
                    foreach (var segReq in proxy.SegmentRequirement)
                    {
                        SegmentRequirements.Add(new SegmentRequirement(segReq)); // throws InvalidMessageException
                    }
                }

                // Read scheduling parameters
                if (proxy.SchedulingParameters != null)
                {
                    try
                    {
                        // Expecting a data record in an XML node array
                        SchedulingParameters = (SXml.XmlNode[])proxy.SchedulingParameters;
                    }
                    catch (InvalidCastException e)
                    {
                        throw new XNeut.InvalidMessageException("Unexpected type of scheduling parameters", e);
                    }
                }
            }
            catch (NullReferenceException e)
            {
                var msg = string.Format("Failed to read ProductionRequest {0} - something required is missing", TryGetIdString());
                throw new XNeut.InvalidMessageException(msg, e);
            }
            catch (XNeut.InvalidMessageException e)
            {
                var msg = string.Format("Failed to read ProductionRequest {0}: {1}", TryGetIdString(), e.Message);
                throw new XNeut.InvalidMessageException(msg, e);
            }
        }

        /// <summary>
        /// Identifier.
        /// </summary>
        public IdentifierType Identifier
        {
            get;
            set;
        }

        /// <summary>
        /// Hierarchy scope.
        /// </summary>
        public HierarchyScope HierarchyScopeObj
        {
            get;
            set;
        }

        /// <summary>
        /// Enclosed segment requirements.
        /// </summary>
        public List<SegmentRequirement> SegmentRequirements
        {
            get;
            set;
        }

        /// <summary>
        /// Scheduling parameters.
        /// </summary>
        public object SchedulingParameters
        {
            get;
            set;
        }

        /// <summary>
        /// Generates an XML proxy from the object.
        /// </summary>
        /// <param name="extraTypes">Any extra types utilised in serialisation.</param>
        /// <returns>Proxy.</returns>
        internal XsdNs.ProductionRequestType ToXmlProxy(out ICollection<Type> extraTypes)
        {
            object parametersProxy = null;
            
            // Scheduling parameters specified?
            if (SchedulingParameters != null)
            {
                parametersProxy = SchedulingParameters;

                // Assuming that the parameters use an extra type
                extraTypes = new List<Type> { SchedulingParameters.GetType() };
            }
            else
            {
                // No extra types used
                extraTypes = new List<Type>();
            }

            XsdNs.IdentifierType identifier = null;

            if (Identifier != null)
            {
                identifier = new XsdNs.IdentifierType();
                Identifier.PopulateXmlProxy(identifier);
            }

            return new XsdNs.ProductionRequestType()
            {
                ID = identifier, // Can be null
                HierarchyScope = HierarchyScopeObj?.ToXmlProxy(), // Can be null
                SegmentRequirement = BuildSegmentRequirementsForProxy(),
                SchedulingParameters = parametersProxy
            };
        }

        private XsdNs.SegmentRequirementType[] BuildSegmentRequirementsForProxy()
        {
            var retval = new XsdNs.SegmentRequirementType[SegmentRequirements.Count];

            for (int a = 0; a < SegmentRequirements.Count; ++a)
            {
                retval[a] = SegmentRequirements[a].ToXmlProxy();
            }
            
            return retval;
        }

        private string TryGetIdString()
        {
            return Identifier == null ? "[Unknown ID]" : Identifier.Value;
        }
    }
}
