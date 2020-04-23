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

namespace Cocop.MessageSerialiser.Biz
{
    /// <summary>
    /// Specifies how material should be used.
    /// </summary>
    public enum MaterialUseType
    {
        /// <summary>
        /// Other.
        /// </summary>
        Other = 0,
        /// <summary>
        /// Produced.
        /// </summary>
        Produced = 1,
        /// <summary>
        /// Consumed.
        /// </summary>
        Consumed = 2,
        /// <summary>
        /// Can be consumed.
        /// </summary>
        Consumable = 3,
        /// <remarks />
        Replaced_Assetn = 4, // "Replaced Assetn" in XML
        /// <remarks />
        Replacement_Asset = 5, // "Replacement Asset" in XML
        /// <remarks />
        Sample = 6,
        /// <remarks />
        Resurned_Sample = 7, // "Resurned Sample" in XML
        /// <remarks />
        Carrier = 8,
        /// <remarks />
        Returned_Carrier = 9 // "Returned Carrier" in XML
    }

    /// <summary>
    /// Represents the equipment element level.
    /// </summary>
    public enum EquipmentElementLevelType
    {
        /// <remarks />
        Other = 0,
        /// <remarks />
        Enterprise = 1,
        /// <remarks />
        Site = 2,
        /// <remarks />
        Area = 3,
        /// <remarks />
        ProcessCell = 4,
        /// <remarks />
        Unit = 5,
        /// <remarks />
        ProductionLine = 6,
        /// <remarks />
        WorkCell = 7,
        /// <remarks />
        ProductionUnit = 8,
        /// <remarks />
        StorageZone = 9,
        /// <remarks />
        StorageUnit = 10,
        /// <remarks />
        WorkCenter = 11,
        /// <remarks />
        WorkUnit = 12,
        /// <remarks />
        EquipmentModule = 13,
        /// <remarks />
        ControlModule = 14
    }
}
