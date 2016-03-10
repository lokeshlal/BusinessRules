using System;
using System.IO;
using System.Xml.Serialization;

namespace BusinessRules.Common
{
    public class Serializer
    {
        public static string Serialize(object o)
        {
            var xs = new XmlSerializer(o.GetType());
            var xml = new StringWriter();
            xs.Serialize(xml, o);
            return Convert.ToString(xml);
        }
        public static T Deserialize<T>(string xml)
        {
            var xs = new XmlSerializer(typeof(T));
            return (T)xs.Deserialize(new StringReader(xml));
        }
    }
}
