using System;
using System.Collections.Generic;
using System.Linq;
using CalDAV.Core.Method_Extensions;
using ICalendar.Calendar;
using TreeForXml;

namespace CalDAV.Core
{
    /// <summary>
    ///     THis class contain the logic for processing a
    ///     REPORT Request.
    /// </summary>
    public class ReportMethods : IReportMethods
    {
        public ReportMethods(string userEmail, string collectionName)
        {
            UserEmail = userEmail;
            CollectionName = collectionName;
        }

        private string UserEmail { get; set; }
        private string CollectionName { get; set; }


        public string ExpandProperty()
        {
            throw new NotImplementedException();
        }

        public string CalendarQuery(IXMLTreeStructure xmlDoc)
        {
            ///the the calendar-data node to know the data that
            /// should ne returned
            IXMLTreeStructure calendarData;
            xmlDoc.GetChildAtAnyLevel("calendar-data", out calendarData);

            ///get the filters to be applied
            IXMLTreeStructure componentFilter;
            xmlDoc.GetChildAtAnyLevel("filter", out componentFilter);


            Dictionary<string, string> userResources;
            var fileM = new FileSystemManagement();
            fileM.GetAllCalendarObjectResource(out userResources);
            var userCalendars = userResources.ToDictionary(userResource => userResource.Key,
                userResource => VCalendar.Parse(userResource.Value));

            var filteredCalendars = userCalendars.Where(x => x.Value.FilterResource(xmlDoc));
            ToXmlString(filteredCalendars, calendarData);
            return "";
        }

        /// <summary>
        ///     Take the calendar that passed the filter and
        ///     create the multi-status xml.
        /// </summary>
        /// <param name="resources">The resources to be returned</param>
        /// <param name="calendarData">
        ///     When used in a calendaring REPORT request, the CALDAV:calendar-data XML
        ///     element specifies which parts of calendar object resources need to be returned in the
        ///     response.If the CALDAV:calendar-data XML element doesn't contain any
        ///     CALDAV:comp element, calendar object resources will be returned in their entirety.
        /// </param>
        /// <returns>The string representation of the multi-status Xml with the results.</returns>
        public string ToXmlString(IEnumerable<KeyValuePair<string, VCalendar>> resources, IXMLTreeStructure calendarData)
        {
            return "";
        }
    }
}