using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TreeForXml;

namespace CalDAV.Utils.XML_Processors
{
    public class XMLParsers
    {
        public static Dictionary<string, List<string>> XMLMKCalendarParser(string doc)
        {
            var xml = XDocument.Parse(doc);
            XNamespace ns = "DAV:";
            var properties = xml.Root.Element(ns + "set").Element(ns + "prop").Elements()
                .ToDictionary(x => x.Name.LocalName, x =>
                {
                    if (x.Descendants().Any())
                    {
                        return new List<string>(x.Descendants().Select(y => (string) y.Attribute("name")));
                    }
                    return new List<string> {x.Value};
                });

            return properties;
        }


        public static IXMLTreeStructure GenericParser(string doc)
        {
            var xml = XDocument.Parse(doc);

            var output = xmlWalker(xml.Root);
            return output;
        }

        private static IXMLTreeStructure xmlWalker(XElement node)
        {
            var output = new XmlTreeStructure(node.Name.LocalName, null)
            {
                Namespaces = node.Attributes().Where(x => x.IsNamespaceDeclaration).
                    ToDictionary(x => x.Name.LocalName, x => x.Value),
                Value = node.Value
            };
            foreach (var attribute in node.Attributes().Where(x => !output.Namespaces.Keys.Contains(x.Name.LocalName)))
            {
                output.AddAttribute(attribute.Name.LocalName, attribute.Value);
            }
            var descendants = node.Elements();
            var descNodes = node.DescendantNodes();
            foreach (var descendant in descendants)
            {
                output.AddChild(xmlWalker(descendant));
            }
            return output;
        }
    }
}