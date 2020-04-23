//
// Please make sure to read and understand the files README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 4/2018
// Last modified: 4/2020

using System;
using SysColl = System.Collections.Generic;
using SXml = System.Xml;

// Using a "neutral" namespace. This namespace does not refer to any other namespace of
// the project, and all of the other namespaces can refer to it. This way, there are no
// bi-directional dependencies between namespaces.
namespace Cocop.MessageSerialiser.Biz.Neutral
{
    /// <summary>
    /// Contains static members that help XML processing.
    /// </summary>
    internal static class Helper
    {
        // These are used to cache serialisers. The framework supposedly does not always
        // cache it, at least if overriding attributes, which reduces performance. Also,
        // the serialiser object is supposedly thread-safe.
        private static SerializerCache m_serialiserCache = new SerializerCache();

        
        /// <summary>
        /// Serialises an object to XML.
        /// </summary>
        /// <param name="obj">The object to be serialised.</param>
        /// <param name="extraTypes">Any extra types needed in serialisation.</param>
        /// <returns>XML data.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the serialisation fails.</exception>
        internal static byte[] ToXmlBytes(object obj, SysColl.ICollection<Type> extraTypes)
        {
            // Creating an XmlTextReader from the XML document
            using (var xmlStream = new System.IO.MemoryStream())
            {
                using (var xmlWriter = new SXml.XmlTextWriter(xmlStream, System.Text.Encoding.UTF8))
                {
                    var extraTypesArr = new Type[extraTypes.Count];
                    extraTypes.CopyTo(extraTypesArr, 0);

                    // Serialising
                    var serializer = m_serialiserCache.GetSerializer(obj.GetType(), extraTypesArr);
                    serializer.Serialize(xmlWriter, obj);
                    
                    // Returning a byte array
                    return xmlStream.ToArray();
                }
            }
        }
        
        /// <summary>
        /// Deserialises an object from XML.
        /// </summary>
        /// <param name="type">Proxy class type.</param>
        /// <param name="xmlBytes">XML data.</param>
        /// <returns>Deserialised proxy object.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the serialisation fails.</exception>
        internal static object DeserialiseFromXml(Type type, byte[] xmlBytes)
        {
            var serializer = m_serialiserCache.GetSerializer(type, new Type[0]);

            // Creating an XmlTextReader from the XML document
            using (var xmlStream = new System.IO.MemoryStream(xmlBytes))
            {
                using (var reader = SXml.XmlReader.Create(xmlStream))
                {
                    // Deserialising
                    return serializer.Deserialize(reader);
                }
            }
        }


        #region Parsing of datatypes
        
        /// <summary>
        /// Converts a boolean to XML boolean.
        /// </summary>
        /// <param name="b">Input.</param>
        /// <returns>XML boolean.</returns>
        internal static string BoolToString(bool b)
        {
            return SXml.XmlConvert.ToString(b);
        }

        /// <summary>
        /// Converts an XML boolean string to a boolean value.
        /// </summary>
        /// <param name="s">Input.</param>
        /// <returns>Boolean.</returns>
        /// <exception cref="ArgumentException">Thrown if parsing fails.</exception>
        internal static bool BoolFromString(string s)
        {
            return (bool)TryParse((a) => // Throws ArgumentException
            {
                return SXml.XmlConvert.ToBoolean(a);
            },
            "boolean", s);
        }

        /// <summary>
        /// Converts a double to XML double.
        /// </summary>
        /// <param name="d">Input.</param>
        /// <returns>XML double.</returns>
        internal static string DoubleToString(double d)
        {
            return SXml.XmlConvert.ToString(d);
        }

        /// <summary>
        /// Converts an XML double string to a double value.
        /// </summary>
        /// <param name="s">Input.</param>
        /// <returns>Double.</returns>
        /// <exception cref="ArgumentException">Thrown if parsing fails.</exception>
        internal static double DoubleFromString(string s)
        {
            return (double)TryParse((a) => // Throws ArgumentException
            {
                return SXml.XmlConvert.ToDouble(a);
            },
            "double", s);
        }

        /// <summary>
        /// Converts a long to XML long.
        /// </summary>
        /// <param name="l">Input.</param>
        /// <returns>XML long.</returns>
        internal static string LongToString(long l)
        {
            return SXml.XmlConvert.ToString(l);
        }

        /// <summary>
        /// Converts an XML long value to a long.
        /// </summary>
        /// <param name="s">Input.</param>
        /// <returns>Long.</returns>
        /// <exception cref="ArgumentException">Thrown if parsing fails.</exception>
        internal static long LongFromString(string s)
        {
            return (long)TryParse((a) => // Throws ArgumentException
            {
                return SXml.XmlConvert.ToInt64(a);
            },
            "long", s);
        }

        /// <summary>
        /// Converts an int to XML int.
        /// </summary>
        /// <param name="i">Input.</param>
        /// <returns>XML int.</returns>
        internal static string IntToString(int i)
        {
            return SXml.XmlConvert.ToString(i);
        }

        /// <summary>
        /// Converts an XML int to an int value.
        /// </summary>
        /// <param name="s">Input.</param>
        /// <returns>Int.</returns>
        /// <exception cref="ArgumentException">Thrown if parsing fails.</exception>
        internal static int IntFromString(string s)
        {
            return (int)TryParse((a) => // Throws ArgumentException
            {
                return SXml.XmlConvert.ToInt32(a);
            },
            "Int32", s);
        }

        /// <summary>
        /// Considers the kind of DateTime. If DateTime kind is known, it is
        /// always converted to UTC. If the kind is unknown, it will remain
        /// unspecified.
        /// </summary>
        /// <returns>DateTime with kind and time zone considered.</returns>
        internal static DateTime DateTimeToUtcIfPossible(DateTime dt)
        {
            switch (dt.Kind)
            {
                case DateTimeKind.Utc:
                case DateTimeKind.Unspecified:
                    // UTC is OK. Unspecified is not OK, but this cannot be helped.
                    return dt;

                case DateTimeKind.Local:
                    // To UTC
                    return dt.ToUniversalTime();

                default: // Not expected, because all cases are supposedly covered.
                    throw new ArgumentException("Unexpected DateTime kind " + dt.Kind);
            }
        }

        /// <summary>
        /// Checks that a DateTime value has UTC as the kind.
        /// </summary>
        /// <param name="dt">DateTime value to be checked.</param>
        /// <exception cref="DateTimeException">Thrown if DateTime kind is not UTC.</exception>
        internal static void ExpectDateTimeIsUtc(DateTime dt)
        {
            if (dt.Kind != DateTimeKind.Utc)
            {
                throw new DateTimeException("DateTime kind must be UTC");
            }
        }

        /// <summary>
        /// Makes sure a DateTime value is suitable for XML serialisation.
        /// </summary>
        /// <param name="dt">DateTime value to check.</param>
        /// <returns>DateTime, converted if needed.</returns>
        /// <exception cref="DateTimeException">Thrown if <paramref name="dt"/> is not in UTC.</exception>
        internal static DateTime DateTimeForSerialisation(DateTime dt)
        {
            ExpectDateTimeIsUtc(dt); // throws DateTimeException
            return dt;
        }

        // Delegate for the generic TryParse function
        private delegate object ParserDelegate(string inputString);

        // Generic parsing function to re-use error handling
        private static object TryParse(ParserDelegate del, string typeName, string inputString)
        {
            try
            {
                return del(inputString);
            }
            catch (ArgumentException e) // Will also catch ArgumentNullException
            {
                var msg = BuildParsingErrorMessage(typeName, inputString);
                throw new ArgumentException(msg, e);
            }
            catch (FormatException e)
            {
                var msg = BuildParsingErrorMessage(typeName, inputString);
                throw new ArgumentException(msg, e);
            }
            catch (OverflowException e)
            {
                var msg = BuildParsingErrorMessage(typeName, inputString);
                throw new ArgumentException(msg, e);
            }
        }

        private static string BuildParsingErrorMessage(string typeName, string inputString)
        {
            return string.Format("Failed to parse {0} from \"{1}\"", typeName, inputString);
        }

        #endregion Parsing of datatypes


        /// <summary>
        /// Serialisers are cached for a better performance.
        /// </summary>
        private class SerializerCache
        {
            // The framework supposedly does not always cache the serialiser, at least if overriding attributes,
            // which reduces performance. Also, the serialiser object is supposedly thread-safe, so it can be
            // shared among applications.
            
            private readonly object m_lockObject = new object();
            private readonly SysColl.Dictionary<string, SXml.Serialization.XmlSerializer> m_cache = new SysColl.Dictionary<string, SXml.Serialization.XmlSerializer>();

            public SerializerCache()
            {
                // Empty ctor body
            }

            public SXml.Serialization.XmlSerializer GetSerializer(Type type, Type[] extraTypes)
            {
                //var key = type.FullName;
                var key = BuildKey(type, extraTypes);
                
                // The creation of a serialiser can take long, but no can do.
                // This will then block any other users.
                lock (m_lockObject)
                {
                    if (!m_cache.ContainsKey(key))
                    {
                        // Creating the serialiser object
                        m_cache[key] = CreateSerialiser(type, extraTypes);
                    }

                    return m_cache[key];
                }
            }

            private static SXml.Serialization.XmlSerializer CreateSerialiser(Type type, Type[] extraTypes)
            {
                // No overrides applied for B2MML
                return new SXml.Serialization.XmlSerializer(type, extraTypes);
            }

            private string BuildKey(Type type, Type[] extraTypes)
            {
                // Add each name in typenames
                var typenames = new SysColl.SortedSet<string>
                {
                    type.FullName
                };

                foreach (Type t in extraTypes)
                {
                    typenames.Add(t.FullName);
                }

                // Generate the key from names
                var sb = new System.Text.StringBuilder();

                foreach (string s in typenames)
                {
                    sb.Append(s);
                    sb.Append(";");
                }

                return sb.ToString();
            }
        }
    }
}
