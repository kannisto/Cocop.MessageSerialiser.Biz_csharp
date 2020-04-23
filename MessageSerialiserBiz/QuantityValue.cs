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
    /// Represents a quantity value.
    /// <para>The respective XML schema type is loose and flexible in terms
    /// of validating the string presentation of the quantity value.
    /// Therefore, the application developers
    /// *must* take care that the quantity string is encoded and
    /// parsed correctly. Fortunately, this class provides help
    /// for utilising some common data types.</para>
    /// <para>Use the <c>DataType</c> property to indicate the type of the raw quantity
    /// string. Please note that this class does not validate the
    /// combination of DataType and the format of the quantity string
    /// in any way. However, certain constructors accept only a certain
    /// type for the value (such as <c>QuantityValue(double val)</c>). These
    /// constructors are safe to use, because they not only encode
    /// the value for XML but also set the DataType respectively.</para>
    /// <para>There is also support for parsing the quantity string for
    /// certain data types. This is realised with methods, such as
    /// <c>TryParseValueAsXmlDouble()</c>.</para>
    /// </summary>
    public sealed class QuantityValue
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="val">Value.</param>
        public QuantityValue(double val)
        {
            RawQuantityString = XNeut.Helper.DoubleToString(val);
            DataType = new DataType(DataType.TypeType.doubleXml);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="val">Value.</param>
        public QuantityValue(bool val)
        {
            RawQuantityString = XNeut.Helper.BoolToString(val);
            DataType = new DataType(DataType.TypeType.booleanXml);
        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="val">Value.</param>
        public QuantityValue(int val)
        {
            RawQuantityString = XNeut.Helper.IntToString(val);
            DataType = new DataType(DataType.TypeType.intXml);
        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="val">Value.</param>
        public QuantityValue(long val)
        {
            RawQuantityString = XNeut.Helper.LongToString(val);
            DataType = new DataType(DataType.TypeType.longXml);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="val">Value serialised to string.</param>
        /// <param name="typ">Data type.</param>
        public QuantityValue(string val, DataType typ)
        {
            if (val == null)
            {
                val = "";
            }

            RawQuantityString = val;
            DataType = typ;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="val">Value serialised to string.</param>
        public QuantityValue(string val)
        {
            if (val == null)
            {
                val = "";
            }

            RawQuantityString = val;
            DataType = null;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="proxy">XML proxy.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        internal QuantityValue(XsdNs.QuantityValueType proxy)
        {
            // Quantity string
            if (proxy.QuantityString == null)
            {
                throw new XNeut.InvalidMessageException("Quantity value is required");
            }
            
            RawQuantityString = string.IsNullOrEmpty(proxy.QuantityString.Value) ? "" : proxy.QuantityString.Value;

            // Data type
            if (proxy.DataType != null)
            {
                DataType = new DataType(proxy.DataType); // throws InvalidMessageException
            }
            else
            {
                DataType = null;
            }
            
            // Unit of measure
            UnitOfMeasure = proxy.UnitOfMeasure?.Value;

            // Key
            if (proxy.Key != null)
            {
                Key = new IdentifierType(proxy.Key);
            }
            else
            {
                Key = null;
            }
        }

        /// <summary>
        /// Data type.
        /// </summary>
        public DataType DataType
        {
            get;
            private set;
        }

        /// <summary>
        /// Unit of measure.
        /// </summary>
        public string UnitOfMeasure
        {
            get;
            set;
        }

        /// <summary>
        /// Raw quantity string.
        /// </summary>
        public string RawQuantityString
        {
            get;
            private set;
        }

        /// <summary>
        /// Key to identify the quantity value.
        /// </summary>
        public IdentifierType Key
        {
            get;
            set;
        }

        /// <summary>
        /// Attempts to parse the quantity string as an XML "double".
        /// </summary>
        /// <returns>Value as double.</returns>
        /// <exception cref="InvalidOperationException">Thrown if parsing fails.</exception>
        public double TryParseValueAsXmlDouble()
        {
            return (double)TryParse((a) =>
            {
                return XNeut.Helper.DoubleFromString(a);
            }
            , "double", RawQuantityString);
        }

        /// <summary>
        /// Attempts to parse the quantity string as an XML "boolean".
        /// </summary>
        /// <returns>Value as boolean.</returns>
        /// <exception cref="InvalidOperationException">Thrown if parsing fails.</exception>
        public bool TryParseValueAsXmlBoolean()
        {
            return (bool)TryParse((a) =>
            {
                return XNeut.Helper.BoolFromString(a);
            }
            , "boolean", RawQuantityString);
        }

        /// <summary>
        /// Attempts to parse the quantity string as an XML int.
        /// </summary>
        /// <returns>Value as int.</returns>
        /// <exception cref="InvalidOperationException">Thrown if parsing fails.</exception>
        public int TryParseValueAsXmlInt()
        {
            return (int)TryParse((a) =>
            {
                return XNeut.Helper.IntFromString(a);
            }
            , "int", RawQuantityString);
        }

        /// <summary>
        /// Attempts to parse the quantity string as an XML long.
        /// </summary>
        /// <returns>Value as long.</returns>
        /// <exception cref="InvalidOperationException">Thrown if parsing fails.</exception>
        public long TryParseValueAsXmlLong()
        {
            return (long)TryParse((a) =>
            {
                return XNeut.Helper.LongFromString(a);
            }
            , "long", RawQuantityString);
        }

        // Delegate for generic parsing
        private delegate object ParserDelegate(string s);

        // Generic parser function
        private object TryParse(ParserDelegate del, string dataType, string input)
        {
            try
            {
                return del(RawQuantityString);
            }
            catch (ArgumentException e) // Will also catch ArgumentNullException
            {
                var msg = GetParsingErrorMsg(dataType, input);
                throw new InvalidOperationException(msg, e);
            }
            catch (FormatException e)
            {
                var msg = GetParsingErrorMsg(dataType, input);
                throw new InvalidOperationException(msg, e);
            }
            catch (OverflowException e)
            {
                var msg = GetParsingErrorMsg(dataType, input);
                throw new InvalidOperationException(msg, e);
            }
        }

        private string GetParsingErrorMsg(string dataType, string input)
        {
            return string.Format("Failed to parse {0} from \"{1}\"", dataType, input);
        }

        /// <summary>
        /// Generates an XML proxy from the object.
        /// </summary>
        /// <returns>XML proxy.</returns>
        internal XsdNs.QuantityValueType ToXmlProxy()
        {
            // Create proxy for unit of measure if specified
            XsdNs.UnitOfMeasureType unitOfMeasProxy = UnitOfMeasure == null ? null :
                new XsdNs.UnitOfMeasureType()
                {
                    Value = UnitOfMeasure
                };

            // Create proxy for key if specified
            XsdNs.IdentifierType keyProxy = null;
            
            if (Key != null)
            {
                keyProxy = new XsdNs.IdentifierType();
                Key.PopulateXmlProxy(keyProxy);
            }

            return new XsdNs.QuantityValueType()
            {
                // Data type
                DataType = DataType?.ToXmlProxy(),

                // Quantity string
                QuantityString = new XsdNs.QuantityStringType()
                {
                    Value = RawQuantityString
                },

                // Unit of measure
                UnitOfMeasure = unitOfMeasProxy,

                // Key
                Key = keyProxy
            };
        }
    }
}
