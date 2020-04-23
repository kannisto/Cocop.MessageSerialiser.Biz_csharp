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
    /// Represents an identifier.
    /// </summary>
    public sealed class IdentifierType
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="v">Value.</param>
        public IdentifierType(string v)
        {
            // The type is normalizedString -> remove whitespaces
            Value = v.Trim();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="proxy">XML proxy.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        internal IdentifierType(XsdNs.IdentifierType proxy)
        {
            // The type is normalizedString -> remove whitespaces
            Value = proxy.Value == null ? "" : proxy.Value.Trim();
        }

        /// <summary>
        /// The actual identifier.
        /// </summary>
        public string Value
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Populates an XML proxy from the object.
        /// </summary>
        /// <param name="proxy">The proxy to be populated. This is necessary, because the
        /// populated proxy is of a base type inapplicable in serialisation.</param>
        internal void PopulateXmlProxy(XsdNs.IdentifierType proxy)
        {
            proxy.Value = Value;
        }
    }
}
