using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EmailSender
{
    /// <summary>
    /// Reads the config files
    /// </summary>
    class ConfigReader
    {
        /// <summary>
        /// Reads xml files and return serialized objects
        /// </summary>
        /// <param name="path">Path of config file</param>
        /// <param name="type">Type of serializable object</param>
        /// <returns>returns serialized of object of xml</returns>
        public static object Read(string path, Type type)
        {
            object serializedObject = null;
            string xmlPath = path;
            try
            {
                XmlSerializer serializer = new XmlSerializer(type);
                using (StreamReader reader = new StreamReader(xmlPath))
                {
                    try
                    {
                        serializedObject = serializer.Deserialize(reader);
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("ERROR :Unable to deserialize the xml\nException :" + ex.Message);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("ERROR :Unable to read xml\nException :" + ex.Message);
            }

            return serializedObject;
        }
    }
}
