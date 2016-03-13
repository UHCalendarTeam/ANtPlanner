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

        void AddChild(IXMLTreeStructure child);

        IXMLTreeStructure GetChild(string childName);

        IXMLTreeStructure GetChildAtAnyLevel(string childName);
    }
}
