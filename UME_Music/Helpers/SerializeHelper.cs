using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UME_Music.Helpers
{
    public class SerializeHelper
    {
        public static void ToXmlFile<T>(string filepath, T obj)
        {
            using (var fs = new FileStream(filepath, FileMode.OpenOrCreate))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(fs, obj);
            }
        }

        public static T ParseFromXmlFile<T>(string filepath)
        {
            using (var fs = new FileStream(filepath, FileMode.OpenOrCreate))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(fs);
            }
        }
    }
}
