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

        private string _userId;
        private string _collectionId;

        public CalDavReport(string userId, string collectionId)
        {
            _userId = userId;
            _collectionId = collectionId;
        }

        public string ExpandProperty()
        {
            throw new NotImplementedException();
        }

        public string CalendarQuery(IXMLTreeStructure filters)
        {
            ///the the calendar-data node to know the data that
            /// should ne returned
            IXMLTreeStructure calendarData;
            filters.GetChildAtAnyLevel("calendar-data", out calendarData);

            ///get the filters to be applied
            IXMLTreeStructure componentFilter;
            filters.GetChildAtAnyLevel("filter", out componentFilter);

            var fMan = new FileSystemManagement();
            return "";



        }
    }
}
