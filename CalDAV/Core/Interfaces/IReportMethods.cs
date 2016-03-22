using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TreeForXml;

namespace CalDAV.Core
{
    interface IReportMethods
    {
        string ExpandProperty();
        string CalendarQuery(IXMLTreeStructure filters);
    }
}
