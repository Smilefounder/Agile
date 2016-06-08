using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Agile.Helpers
{
    public class SerializeHelper
    {

        public static string ToXml(object obj)
        {
            var serializer = new XmlSerializer(obj.GetType());
            using (var ms = new MemoryStream())
            {
                serializer.Serialize(ms, obj);
                var xml = Encoding.UTF8.GetString(ms.ToArray());
                return xml;
            }
        }

        public static T ParseFromXml<T>(string xml)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(ms);
            }
        }

        public static string ToJson(object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return json;
        }

        public static T ParseFromJson<T>(string json)
        {
            var entity = JsonConvert.DeserializeObject<T>(json);
            return entity;
        }
    }
}
