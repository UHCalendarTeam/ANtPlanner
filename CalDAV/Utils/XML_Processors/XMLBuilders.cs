using System.Xml.Linq;

//using System.Xml.Linq;

namespace CalDAV.XML_Processors
{
    public class XMLBuilders
    {
        public static string XmlBuilder(string name, string namespc, string value)
        {
            XNamespace ns = namespc;
            XName nm = "xmlns";
            var doc = new XElement("C--" + name, new XAttribute("xmlns--C", "urn:ietf:params:xml:ns:caldav"))
            {
                Value = value
            };
            //doc.SetAttributeValue(nm, "urn:ietf:params:xml:ns:caldav");
            return doc.ToString().Replace("--", ":");
        }
    }
}