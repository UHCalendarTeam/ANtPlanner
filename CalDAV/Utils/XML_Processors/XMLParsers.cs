using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

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
                        return new List<string>(x.Descendants().Select(y => (string)y.Attribute("name")));
                    }
                    else
                    {
                        return new List<string>() { (string)x.Value };
                    }
                });

            return properties;
        }


        public static IXMLTreeStructure GenericParser(string doc)
        {
            var xml = XDocument.Parse(doc);
            
             var output= xmlWalker(xml.Root);
            return output;
        }

        private static IXMLTreeStructure xmlWalker(XElement node)
        {
            var output = new XMLTreeStructure(node.Name.LocalName);
            output.AddNamespace(node.Name.NamespaceName);
            output.AddValue(node.Value);
            foreach (var attribute in node.Attributes())
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
