using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.Utils.XML_Processors;

namespace CalDAV.CALDAV_Properties.Interfaces
{
    interface IDavProperties
    {
        string DisplayName { get; }

        DateTime CreationDate { get; }

        int GetContentLenght { get; }

        string GetContentType { get; }

        string GetEtag { get; set; }

        DateTime GetLastModified { get; }

        string GetContentLanguage { get; }

        XmlTreeStructure LockDiscovery { get;}

        XmlTreeStructure ResourceType { get; }

        XmlTreeStructure SupportedLock { get; }
    }
}
