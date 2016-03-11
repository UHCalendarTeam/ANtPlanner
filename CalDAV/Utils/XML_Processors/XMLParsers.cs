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
    }
}
