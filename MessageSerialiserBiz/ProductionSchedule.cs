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
    /// Represents a production schedule that can request to realise multiple
    /// production entities.
    /// </summary>
    public sealed class ProductionSchedule
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ProductionSchedule()
        {
            ProductionRequests = new List<ProductionRequest>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="proxy">XML proxy.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        internal ProductionSchedule(XsdNs.ProductionScheduleType proxy)
        {
            ProductionRequests = new List<ProductionRequest>();

            // Reading items from proxy
            try
            {
                if (proxy.ProductionRequest != null)
                {
                    foreach (var requestProxy in proxy.ProductionRequest)
                    {
                        ProductionRequests.Add(new ProductionRequest(requestProxy));
                    }
                }
            }
            catch (NullReferenceException e)
            {
                throw new XNeut.InvalidMessageException("Failed to read ProductionSchedule - something required is missing", e);
            }
        }

        /// <summary>
        /// Enclosed production requests.
        /// </summary>
        public List<ProductionRequest> ProductionRequests
        {
            get;
            set;
        }

        /// <summary>
        /// Generates an XML proxy from the object.
        /// </summary>
        /// <param name="extraTypes">Any extra types needed in serialisation.</param>
        /// <returns>XML proxy.</returns>
        internal XsdNs.ProductionScheduleType ToXmlProxy(out ICollection<Type> extraTypes)
        {
            return new XsdNs.ProductionScheduleType()
            {
                ProductionRequest = BuildProductionRequestsForProxy(out extraTypes)
            };
        }

        private XsdNs.ProductionRequestType[] BuildProductionRequestsForProxy(out ICollection<Type> extraTypes) 
        {
            var extraTypesSet = new HashSet<Type>();
            var retval = new XsdNs.ProductionRequestType[ProductionRequests.Count];

            for (int a = 0; a < ProductionRequests.Count; ++a)
            {
                retval[a] = ProductionRequests[a].ToXmlProxy(out ICollection<Type> extraTypesTemp);

                // Add any new extra types
                extraTypesSet.UnionWith(extraTypesTemp);
            }

            extraTypes = extraTypesSet;
            return retval;
        }
    }
}
