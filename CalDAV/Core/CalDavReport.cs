using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalDAV.Core
{
    /// <summary>
    /// THis class contain the logic for processing a 
    /// REPORT Request.
    /// </summary>
    public class CalDavReport:IReportMethods
    {
        public string ExpandProperty()
        {
            throw new NotImplementedException();
        }

        public string CalendarQuery(List<string> filters)
        {
            throw new NotImplementedException();
        }
    }
}
