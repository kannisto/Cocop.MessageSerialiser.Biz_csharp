//
// Please make sure to read and understand the files README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 5/2018
// Last modified: 4/2020

using System;
using System.Collections.Generic;

namespace TestCommon
{
    /// <summary>
    /// Performs XML validation.
    /// </summary>
    class Validator
    {
        // *** For the XML validation to work, the related schemata must be locally available in the computer. ***
        
        /// <summary>
        /// Indicates which schemata to include in validation.
        /// </summary>
        [Flags]
        public enum SchemaType
        {
            /// <summary>
            /// Include B2MML, which refers to no other schema.
            /// </summary>
            B2mml = 1,
            /// <summary>
            /// Include SWE, which refers to no other schema.
            /// </summary>
            Swe = 2
        }

        private static System.Xml.XmlReaderSettings m_xmlReaderSettings = null;

        private readonly SchemaType m_schemata;

        private List<string> m_validationMessages = new List<string>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sch">The schemata to include in validation.</param>
        public Validator(SchemaType sch)
        {
            m_schemata = sch;
        }

        /// <summary>
        /// Validates the given XML document.
        /// </summary>
        /// <param name="input">XML document as stream.</param>
        /// <param name="ser">Serialiser.</param>
        /// <exception cref="InvalidOperationException">Thrown if the document is invalid.</exception>
        public void Validate(System.IO.Stream input, Type type)
        {
            var settings = GetSettings();

            // Adding the event handler of this object
            settings.ValidationEventHandler += Settings_ValidationEventHandler;
            
            // Clearing validation messages
            m_validationMessages.Clear();
            
            // Declaring a prefix for the XLink namespace explicitly.
            // This is necessary to support scheduling parameters of type
            // Item_DataRecord from the library "Cocop.MessageSerialiser.Meas".
            var nametable = new System.Xml.NameTable();
            var xmlNsManager = new System.Xml.XmlNamespaceManager(nametable);
            xmlNsManager.AddNamespace("xlink", "http://www.w3.org/1999/xlink");

            // Declaring a context object to pass the namespace manager to the parser
            var parserContext = new System.Xml.XmlParserContext(null, xmlNsManager, null, System.Xml.XmlSpace.None);

            using (var reader = System.Xml.XmlReader.Create(input, settings, parserContext))
            {
                // Deserialising the document
                var serializer = new System.Xml.Serialization.XmlSerializer(type);
                serializer.Deserialize(reader);
            }

            // Removing the event handler of this object (the event handler would 
            // otherwise cause conflicts when multiple documents are validated)
            settings.ValidationEventHandler -= Settings_ValidationEventHandler;
            
            if (m_validationMessages.Count > 0)
            {
                var msg = string.Format("The XML document has {0} errors. First error: {1}", m_validationMessages.Count, m_validationMessages[0]);
                throw new InvalidOperationException(msg);
            }
        }

        private System.Xml.XmlReaderSettings GetSettings()
        {
            if (m_xmlReaderSettings == null)
            {
                // Setting up validation settings
                var settings = new System.Xml.XmlReaderSettings
                {
                    CloseInput = false,
                    ValidationType = System.Xml.ValidationType.Schema,
                    XmlResolver = new System.Xml.XmlUrlResolver()
                    {
                        CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.CacheIfAvailable)
                    }
                };
                settings.ValidationFlags |= System.Xml.Schema.XmlSchemaValidationFlags.ProcessSchemaLocation;
                settings.ValidationFlags |= System.Xml.Schema.XmlSchemaValidationFlags.ReportValidationWarnings;
                settings.ValidationFlags |= System.Xml.Schema.XmlSchemaValidationFlags.ProcessIdentityConstraints;

                // Including only the required schemata to speed up validation.
                if (m_schemata.HasFlag(SchemaType.B2mml))
                {
                    settings.Schemas.Add(null, TestHelper.SchemaFolderB2mml + @"\b2mml\B2MML-V0600-ProductionSchedule.xsd");
                }
                if (m_schemata.HasFlag(SchemaType.Swe))
                {
                    settings.Schemas.Add(null, TestHelper.SchemaFolderRef + @"\swe\swe.xsd");
                }

                m_xmlReaderSettings = settings;
            }

            return m_xmlReaderSettings;
        }

        private void Settings_ValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs args)
        {
            // Checking if the message results from a bug (?) in the .NET libs.
            // The message would be, e.g.,
            // Validating file C:\lokaali\lokaali_ws\vs2015-cocop\MessageSerialiser\XmlValidationDev\testfiles\valid1.xml...
            // The document is not valid.Got 1 validation message(s):
            // 1: Error: The global element 'http://foo.com/xyz/myelement' has already been declared.
            if (args.Message.ToLower().Contains("the global element") &&
                args.Message.ToLower().Contains("has already been declared"))
            {
                return;
            }
            else
            {
                // Adding event information to validation messages
                var msg = string.Format("{0}: {1}", args.Severity.ToString(), args.Message);
                m_validationMessages.Add(msg);
            }
        }
    }
}
