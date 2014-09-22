using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace anhnv.csharp.utils.Types
{
    class XmlConverter
    {
        public static T XmlToObject<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            var byteArray = Encoding.UTF8.GetBytes(xml);
            var fs = new MemoryStream(byteArray);
            var t = (T)serializer.Deserialize(fs);
            return t;
        }

        public static string ObjectToXml<T>(T t)
        {
            var serializer = new XmlSerializer(typeof(T));
            var mStream = new MemoryStream();
            serializer.Serialize(mStream, t);
            //mStream.Position = 0;
            mStream.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[mStream.Length];
            mStream.Read(buffer, 0, (int)mStream.Length);

            var xmlBody = new XmlDocument();
            xmlBody.LoadXml(Encoding.UTF8.GetString(buffer));

            foreach (XmlNode tmpNode in xmlBody)
            {
                if (tmpNode.NodeType == XmlNodeType.XmlDeclaration)
                {
                    xmlBody.RemoveChild(tmpNode);
                }
            }
            return xmlBody.OuterXml;
        }

        /// <summary>
        /// Convert 1 đối tượng thành XML với Prefix và Namespace tùy chọn
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">Đối tượng</param>
        /// <param name="prefix"></param>
        /// <param name="namespaceUrl"></param>
        /// <returns></returns>
        public static string ObjectToXml<T>(T t, string prefix, string namespaceUrl)
        {
            var serializer = new XmlSerializer(typeof(T));
            var mStream = new MemoryStream();
            var names = new XmlSerializerNamespaces();
            names.Add(prefix, namespaceUrl);
            serializer.Serialize(mStream, t, names);
            //mStream.Position = 0;
            mStream.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[mStream.Length];
            mStream.Read(buffer, 0, (int)mStream.Length);
            var xmlBody = new XmlDocument();
            xmlBody.LoadXml(Encoding.UTF8.GetString(buffer));

            foreach (XmlNode tmpNode in xmlBody)
            {
                if (tmpNode.NodeType == XmlNodeType.XmlDeclaration)
                {
                    xmlBody.RemoveChild(tmpNode);
                }
            }
            return xmlBody.OuterXml;
        }

        /// <summary>
        /// Xóa toàn bộ namespace nếu có của XmlDocument
        /// </summary>
        /// <param name="xmlDocument">XML String</param>
        /// <returns>Xml String đã được loại bỏ Namespace</returns>
        public static string RemoveAllNamespaces(string xmlDocument)
        {
            XElement xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Parse(xmlDocument));
            return xmlDocumentWithoutNs.ToString();
        }

        /// <summary>
        /// Hàm đệ quy xóa namespace của 1 phần tử XML
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns>Xml đã được loại bỏ Namespace</returns>
        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                var xElement = new XElement(xmlDocument.Name.LocalName) {Value = xmlDocument.Value};

                foreach (var attribute in xmlDocument.Attributes())
                    xElement.Add(attribute);

                return xElement;
            }
            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(RemoveAllNamespaces));
        }
    }
}
