using System.Collections.Generic;
using DataLayer;
using ICalendar.Calendar;
using Microsoft.AspNet.Http;
using TreeForXml;

namespace CalDAV.Core
{
    internal interface IReportMethods
    {
        string ExpandProperty();

        /// <summary>
        ///     Process the REPORT request and send back the
        ///     result of the applied operations.
        /// </summary>
        /// <param name="context">Send the HttpContext from the controller. THis
        /// contains the response so can be modified and the request to take
        /// useful data from it.</param>
        /// <returns>The data for the body of the response of the request.</returns>
        string ProcessRequest(HttpContext context);

        /// <summary>
        ///     Apply the calendar-query opertation to a
        ///     user calendars.
        /// </summary>
        /// <param name="xmlBody">The xml send in the body of the request.</param>
        /// <param name="storageManagement">An instance of the FileSystemManagment of the requested collection.</param>
        /// <returns></returns>
        string CalendarQuery(IXMLTreeStructure xmlBody, string collectionUrl);

        /// <summary>
        ///     Take the calendar that passed the filter and
        ///     create the multi-status xml.
        /// </summary>
        /// <param name="resources">The resources to be returned</param>
        /// <param name="calendarData">
        ///     When used in a calendaring REPORT request, the CALDAV:calendar-data XML
        ///     element specifies which parts of calendar object resources need to be returned in the
        ///     response.If the CALDAV:calendar-data XML element doesn't contain any
        ///     CALDAV:comp element, calendar object resources will be returned in their entirety./param>
        ///     <returns>The string representation of the multi-status Xml with the results.</returns>
        string ReportResponseBuilder(IEnumerable<KeyValuePair<string, VCalendar>> resources,
            IXMLTreeStructure calendarData);
    }
}