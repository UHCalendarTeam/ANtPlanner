using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TreeForXml;

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

        public string CalendarQuery(IXMLTreeStructure filters)
        {
            throw new NotImplementedException();
        }
    }
}
