using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalDAV.Utils.XML_Processors
{
    public interface IXMLTreeStructure
    {
         string NodeName { get;  }

        List<string> Namespaces{get;}

        List<IXMLTreeStructure> Children { get; }

        Dictionary<string, string> Attributes { get; }

        string Value { get; }

        IXMLTreeStructure AddChild(IXMLTreeStructure child);

        IXMLTreeStructure GetChild(string childName);

        IXMLTreeStructure GetChildAtAnyLevel(string childName);


        IXMLTreeStructure AddNamespace(string ns);

        IXMLTreeStructure AddAttribute(string name, string value);


        IXMLTreeStructure AddValue(string value);
    }
}
