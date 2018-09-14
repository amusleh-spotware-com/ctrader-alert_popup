using System;
using System.IO;
using System.Xml.Serialization;

namespace cAlgo.API.Alert.Types
{
    public class Serializer
    {
        #region Methods

        /// <summary>
        /// Serializes an object of type T to XML and returns the XML data in string
        /// </summary>
        /// <returns>string</returns>
        public static string Serialize<T>(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            string xml = string.Empty;

            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, obj);

                xml = textWriter.ToString();
            }

            if (string.IsNullOrEmpty(xml))
            {
                throw new InvalidOperationException("Couldn't serialize the pipe packet");
            }

            return xml;
        }

        /// <summary>
        /// Deserializes XML data to an object of type T
        /// </summary>
        /// <param name="xml">XML data</param>
        /// <returns>T<T></returns>
        public static T Derserialize<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (StringReader textReader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(textReader);
            }
        }

        #endregion Methods
    }
}