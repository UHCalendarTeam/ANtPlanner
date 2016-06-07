using System.Collections.Generic;
using System.Threading.Tasks;
using ICalendar.Calendar;
using Microsoft.AspNetCore.Http;
using TreeForXml;

namespace CalDAV.Core
{
    /// <summary>
    ///     This interface defines the necessary methods
    ///     to handle the HTTP REPORT method.
    /// </summary>
    public interface ICollectionReport
    {
        //string ExpandProperty();

        /// <summary>
        ///     Process the REPORT request and send back the
        ///     result of the applied operations.
        /// </summary>
        /// <param name="context">
        ///     Send the HttpContext from the controller. THis
        ///     contains the response so can be modified and the request to take
        ///     useful data from it.
        /// </param>
        /// <returns>The data for the body of the response of the request.</returns>
        Task ProcessRequest(HttpContext context);

        /// <summary>
        ///     Apply the calendar-query operation
        ///     to a collection.
        /// </summary>
        /// <param name="xmlBody">The xml send in the body of the request.</param>
        /// <param name="collectionUrl"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        Task CalendarQuery(IXMLTreeStructure xmlBody, string collectionUrl, HttpContext httpContext);

        /// <summary>
        ///     The CALDAV:calendar-multiget REPORT is used to retrieve specific calendar object resources from within a
        ///     collection, if the Request-URI is a collection, or to retrieve a specific calendar object resource, if the
        ///     Request-URI is a calendar object resource. This report is similar to the CALDAV:calendar-query REPORT
        ///     (see Section 7.8), except that it takes a list of DAV:href elements, instead of a CALDAV:filter element, to
        ///     determine which calendar object resources to return
        /// </summary>
        /// <returns></returns>
        Task CalendarMultiget(IXMLTreeStructure xmlBody, HttpContext httpContext);

        /// <summary>
        ///     Take the calendar that passed the filters and
        ///     create the multi-status xml that has to be send
        ///     back in the response.
        /// </summary>
        /// <param name="resources">The resources to be returned</param>
        /// <param name="calendarData">
        ///     When used in a calendaring REPORT request, the CALDAV:calendar-data XML
        ///     element specifies which parts of calendar object resources need to be returned in the
        ///     response.If the CALDAV:calendar-data XML element doesn't contain any
        ///     CALDAV:comp element, calendar object resources will be returned in their entirety./param>
        ///     <returns>The string representation of the multi-status Xml with the results.</returns>
        /// </param>
        /// <param name="httpContext"></param>
        Task ReportResponseBuilder(IEnumerable<KeyValuePair<string, VCalendar>> resources,
            IXMLTreeStructure calendarData, HttpContext httpContext);
    }
}