using System;
using TreeForXml;

namespace DataLayer
{
    public interface IDavProperties
    {
        string DisplayName { get; }

        DateTime CreationDate { get; }

        int GetContentLength { get; }

        string GetContentType { get; }

        string GetEtag { get; set; }

        DateTime GetLastModified { get; }

        string GetContentLanguage { get; }

        XmlTreeStructure LockDiscovery { get; }

        XmlTreeStructure ResourceType { get; }

        XmlTreeStructure SupportedLock { get; }
    }
}