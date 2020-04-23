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
    /// Represents a material-related requirement.
    /// </summary>
    public sealed class MaterialRequirement
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MaterialRequirement()
        {
            MaterialDefinitionIdentifiers = new List<IdentifierType>();
            MaterialLotIdentifiers = new List<IdentifierType>();
            Quantities = new List<QuantityValue>();
            AssemblyRequirements = new List<MaterialRequirement>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="proxy">XML proxy.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        internal MaterialRequirement(XsdNs.MaterialRequirementType proxy)
            : this() // Call the default constructor
        {
            if (proxy.MaterialDefinitionID != null)
            {
                foreach (var idProxy in proxy.MaterialDefinitionID)
                {
                    MaterialDefinitionIdentifiers.Add(new IdentifierType(idProxy)); // throws InvalidMessageException
                }
            }

            if (proxy.MaterialLotID != null)
            {
                foreach (var idProxy in proxy.MaterialLotID)
                {
                    MaterialLotIdentifiers.Add(new IdentifierType(idProxy)); // throws InvalidMessageException
                }
            }

            if (proxy.MaterialUse != null)
            {
                MaterialUse = new MaterialUse(proxy.MaterialUse); // throws InvalidMessageException
            }

            if (proxy.Quantity != null)
            {
                foreach (var qItem in proxy.Quantity)
                {
                    Quantities.Add(new QuantityValue(qItem)); // throws InvalidMessageException
                }
            }

            if (proxy.AssemblyRequirement != null)
            {
                foreach (var arItem in proxy.AssemblyRequirement)
                {
                    AssemblyRequirements.Add(new MaterialRequirement(arItem)); // throws InvalidMessageException
                }
            }
        }

        /// <summary>
        /// Material definition identifiers.
        /// </summary>
        public List<IdentifierType> MaterialDefinitionIdentifiers
        {
            get;
            set;
        }

        /// <summary>
        /// Material lot identifiers.
        /// </summary>
        public List<IdentifierType> MaterialLotIdentifiers
        {
            get;
            set;
        }

        /// <summary>
        /// How the material is to be used.
        /// </summary>
        public MaterialUse MaterialUse
        {
            get;
            set;
        }

        /// <summary>
        /// Enclosed quantities.
        /// </summary>
        public List<QuantityValue> Quantities
        {
            get;
            set;
        }

        /// <summary>
        /// Enclosed material requirements. Use to specify the composition of a material.
        /// </summary>
        public List<MaterialRequirement> AssemblyRequirements
        {
            get;
            set;
        }

        /// <summary>
        /// Generates an XML proxy from the object.
        /// </summary>
        /// <returns>XML proxy.</returns>
        internal XsdNs.MaterialRequirementType ToXmlProxy()
        {
            return new XsdNs.MaterialRequirementType()
            {
                // Material definition identifiers
                MaterialDefinitionID = BuildMaterialDefinitionIdsForProxy(),

                // Material lot identifiers
                MaterialLotID = BuildMaterialLotIdsForProxy(),

                // Material use
                MaterialUse = MaterialUse?.ToXmlProxy(),

                // Quantities
                Quantity = BuildQuantitiesForProxy(),

                // Assembly requirements
                AssemblyRequirement = BuildAssemblyReqsForProxy()
            };
        }

        private XsdNs.MaterialDefinitionIDType[] BuildMaterialDefinitionIdsForProxy()
        {
            var retval = new XsdNs.MaterialDefinitionIDType[MaterialDefinitionIdentifiers.Count];

            for (int a = 0; a < MaterialDefinitionIdentifiers.Count; ++a)
            {
                var idProxy = new XsdNs.MaterialDefinitionIDType();
                MaterialDefinitionIdentifiers[a].PopulateXmlProxy(idProxy);
                retval[a] = idProxy;
            }

            return retval;
        }

        private XsdNs.MaterialLotIDType[] BuildMaterialLotIdsForProxy()
        {
            var retval = new XsdNs.MaterialLotIDType[MaterialLotIdentifiers.Count];

            for (int a = 0; a < MaterialLotIdentifiers.Count; ++a)
            {
                var idProxy = new XsdNs.MaterialLotIDType();
                MaterialLotIdentifiers[a].PopulateXmlProxy(idProxy);
                retval[a] = idProxy;
            }

            return retval;
        }

        private XsdNs.QuantityValueType[] BuildQuantitiesForProxy()
        {
            var retval = new XsdNs.QuantityValueType[Quantities.Count];

            for (int a = 0; a < Quantities.Count; ++a)
            {
                retval[a] = Quantities[a].ToXmlProxy();
            }

            return retval;
        }

        private XsdNs.MaterialRequirementType[] BuildAssemblyReqsForProxy()
        {
            var retval = new XsdNs.MaterialRequirementType[AssemblyRequirements.Count];

            for (int a = 0; a < AssemblyRequirements.Count; ++a)
            {
                retval[a] = AssemblyRequirements[a].ToXmlProxy();
            }

            return retval;
        }
    }
}
