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
    /// Represents a data type.
    /// </summary>
    public sealed class DataType
    {
        private const string SuffixUnCefact = "_UN_CEFACT";
	    private const string SuffixXml = "Xml";

        /// <summary>
        /// Represents a data type. Some types come from UN/CEFACT, whereas others come from the XML standard.
        /// </summary>
        public enum TypeType
        {
            /// <remarks/>
            Other = 0,
            /// <remarks/>
            Amount_UN_CEFACT = 1,
            /// <remarks/>
            BinaryObject_UN_CEFACT = 2,
            /// <remarks/>
            Code_UN_CEFACT = 3,
            /// <remarks/>
            DateTime_UN_CEFACT = 4,
            /// <remarks/>
            Identifier_UN_CEFACT = 5,
            /// <remarks/>
            Indicator_UN_CEFACT = 6,
            /// <remarks/>
            Measure_UN_CEFACT = 7,
            /// <remarks/>
            Numeric_UN_CEFACT = 8,
            /// <remarks/>
            Quantity_UN_CEFACT = 9,
            /// <remarks/>
            Text_UN_CEFACT = 10,
            /// <remarks/>
            stringXml = 11,
            /// <remarks/>
            byteXml = 12,
            /// <remarks/>
            unsignedByteXml = 13,
            /// <remarks/>
            binaryXml = 14,
            /// <remarks/>
            integerXml = 15,
            /// <remarks/>
            positiveIntegerXml = 16,
            /// <remarks/>
            negativeIntegerXml = 17,
            /// <remarks/>
            nonNegativeIntegerXml = 18,
            /// <remarks/>
            nonPositiveIntegerXml = 19,
            /// <remarks/>
            intXml = 20,
            /// <remarks/>
            unsignedIntXml = 21,
            /// <remarks/>
            longXml = 22,
            /// <remarks/>
            unsignedLongXml = 23,
            /// <remarks/>
            shortXml = 24,
            /// <remarks/>
            unsignedShortXml = 25,
            /// <remarks/>
            decimalXml = 26,
            /// <remarks/>
            floatXml = 27,
            /// <remarks/>
            doubleXml = 28,
            /// <remarks/>
            booleanXml = 29,
            /// <remarks/>
            timeXml = 30,
            /// <remarks/>
            timeInstantXml = 31,
            /// <remarks/>
            timePeriodXml = 32,
            /// <remarks/>
            durationXml = 33,
            /// <remarks/>
            dateXml = 34,
            /// <remarks/>
            dateTimeXml = 35,
            /// <remarks/>
            monthXml = 36,
            /// <remarks/>
            yearXml = 37,
            /// <remarks/>
            centuryXml = 38,
            /// <remarks/>
            recurringDayXml = 39,
            /// <remarks/>
            recurringDateXml = 40,
            /// <remarks/>
            recurringDurationXml = 41,
            /// <remarks/>
            NameXml = 42,
            /// <remarks/>
            QNameXml = 43,
            /// <remarks/>
            NCNameXml = 44,
            /// <remarks/>
            uriReferenceXml = 45,
            /// <remarks/>
            languageXml = 46,
            /// <remarks/>
            IDXml = 47,
            /// <remarks/>
            IDREFXml = 48,
            /// <remarks/>
            IDREFSXml = 49,
            /// <remarks/>
            ENTITYXml = 50,
            /// <remarks/>
            ENTITIESXml = 51,
            /// <remarks/>
            NOTATIONXml = 52,
            /// <remarks/>
            NMTOKENXml = 53,
            /// <remarks/>
            NMTOKENSXml = 54,
            /// <remarks/>
            EnumerationXml = 55,
            /// <remarks/>
            SVGXml = 56
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="t">Data type.</param>
        public DataType(TypeType t)
        {
            Type = t;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="proxy">XML proxy.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        internal DataType(XsdNs.DataTypeType proxy)
        {
            if (proxy.Value == null)
            {
                throw new XNeut.InvalidMessageException("If datatype element is present, it must have a value");
            }

            Type = ParseType(proxy.Value); // throws InvalidMessageException
        }

        /// <summary>
        /// Type.
        /// </summary>
        public TypeType Type
        {
            get;
            private set;
        }

        private TypeType ParseType(string raw)
        {
		    // A) other
		    if (raw.Equals(TypeType.Other.ToString()))
		    {
			    return TypeType.Other;
		    }
		
		    // B) Try to parse as an XML type
		    try
		    {
                return (TypeType)Enum.Parse(typeof(TypeType), raw + SuffixXml, ignoreCase: false);
		    }
		    catch (ArgumentException) // Will also catch ArgumentNullException
            { } // Do nothing, try with another
		
		    // C) Try to parse as a UN/CEFACT type
		    try
		    {
                return (TypeType)Enum.Parse(typeof(TypeType), raw + SuffixUnCefact, ignoreCase: false);
		    }
		    catch (ArgumentException e) // Will also catch ArgumentNullException
            {
			    throw new XNeut.InvalidMessageException("Failed to parse datatype from \"" + raw + "\"", e);
		    }
	    }

        private string TypeToString(TypeType t)
        {
            if (t == TypeType.Other)
            {
                // This value requires no additional processing
                return t.ToString();
            }

            // 1) Just convert to string
            var raw = t.ToString();

            // 2 A) Remove suffix of XML types
            if (raw.EndsWith(SuffixXml))
            {
                return RemoveSuffix(raw, SuffixXml);
            }
            // 2 B) Remove suffix of UN/CEFACT types
            else if (raw.EndsWith(SuffixUnCefact))
            {
                return RemoveSuffix(raw, SuffixUnCefact);
            }
            else
            {
                // If this happens, there is a bug somewhere
                throw new InvalidOperationException("Unexpected datatype value \"" + t.ToString() + "\"");
            }
        }

        private string RemoveSuffix(string str, string suffix)
        {
            int substringLen = str.Length - suffix.Length;
            return str.Substring(0, substringLen);
        }

        /// <summary>
        /// Generates an XML proxy from the object.
        /// </summary>
        /// <returns>XML proxy.</returns>
        /// <exception cref="InvalidOperationException">Thrown if there is a type mismatch.</exception>
        internal XsdNs.DataTypeType ToXmlProxy()
        {
            return new XsdNs.DataTypeType()
            {
                Value = TypeToString(Type) // throws InvalidOperationException
            };
        }
    }
}
