using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CalDAV.Core
{
    interface IReportMethods
    {
        string ExpandProperty();
        string CalendarQuery(List<string> filters);
    }
}
