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
using XsdNs = Cocop.MessageSerialiser.Biz.XsdGen;
using XNeut = Cocop.MessageSerialiser.Biz.Neutral;

namespace Cocop.MessageSerialiser.Biz
{
    /// <summary>
    /// Indicates the scope of equipment within the plant hierarchy.
    /// </summary>
    public sealed class HierarchyScope
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eqId">Equipment identifier.</param>
        /// <param name="lev">Equipment element level.</param>
        /// <exception cref="ArgumentException">Thrown if equipment ID is null.</exception>
        public HierarchyScope(IdentifierType eqId, EquipmentElementLevelType lev)
        {
            if (eqId == null || eqId.Value == null)
            {
                throw new ArgumentException("Equipment ID must not be null in hierarchy scope");
            }

            EquipmentIdentifier = eqId;
            EquipmentElementLevel = lev;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="proxy">XML proxy.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        internal HierarchyScope(XsdNs.HierarchyScopeType proxy)
        {
            try
            {
                EquipmentIdentifier = new IdentifierType(proxy.EquipmentID); // throws InvalidMessageException
                string eqLevRaw = proxy.EquipmentElementLevel.Value;
                EquipmentElementLevel = (EquipmentElementLevelType)Enum.Parse(typeof(EquipmentElementLevelType), eqLevRaw);
            }
            catch (ArgumentException e)
            {
                throw new XNeut.InvalidMessageException("Invalid equipment element level", e);
            }
            catch (NullReferenceException e)
            {
                throw new XNeut.InvalidMessageException("Failed to read HierarchyScope - something expected is missing", e);
            }
        }
        
        /// <summary>
        /// Equipment ID.
        /// </summary>
        public IdentifierType EquipmentIdentifier
        {
            get;
            private set;
        }

        /// <summary>
        /// Equipment element level.
        /// </summary>
        public EquipmentElementLevelType EquipmentElementLevel
        {
            get;
            private set;
        }

        /// <summary>
        /// Generates an XML proxy from the object.
        /// </summary>
        /// <returns>XML proxy.</returns>
        internal XsdNs.HierarchyScopeType ToXmlProxy()
        {
            // Create and populate ID proxy
            var idProxy = new XsdNs.EquipmentIDType();
            EquipmentIdentifier.PopulateXmlProxy(idProxy);
            
            return new XsdNs.HierarchyScopeType()
            {
                EquipmentID = idProxy,
                
                EquipmentElementLevel = new XsdNs.EquipmentElementLevelType()
                {
                    Value = EquipmentElementLevel.ToString()
                }
            };
        }
    }
}
