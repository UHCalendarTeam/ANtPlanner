using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.Utils.XML_Processors;

namespace CalDAV.Core.Propfind
{
    interface IPropfindMethods
    {
        string AllPropMethod(XMLTreeStructure propFindBody);

        string PropMethod(XMLTreeStructure propFindBody);

        string PropNameMethod(XMLTreeStructure propFindBody);
    }
}
