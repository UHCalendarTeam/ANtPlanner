using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CalDAV.Utils.XML_Processors
{
    public interface IXMLTreeStructure
    {
         string NodeName { get; set; }

        string MainNamespace { get; set; }

        Dictionary<string, string> Namespaces{get; set; }

        List<IXMLTreeStructure> Children { get; set; }

        Dictionary<string, string> Attributes { get; set; }

        string Value { get; }

        IXMLTreeStructure AddChild(IXMLTreeStructure child);

        IXMLTreeStructure GetChild(string childName);

        IXMLTreeStructure GetChildAtAnyLevel(string childName);


        IXMLTreeStructure AddNamespace(string ns, string nsLocal);

        IXMLTreeStructure AddAttribute(string name, string value);


        IXMLTreeStructure AddValue(string value);
        

         XElement ToXml(IXMLTreeStructure parent);
    }
}
