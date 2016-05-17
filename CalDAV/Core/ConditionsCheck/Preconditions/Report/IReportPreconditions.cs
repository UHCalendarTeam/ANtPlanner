using System;
using System.Collections.Generic;
using System.Xml.Linq;
using ICalendar.Calendar;
using Microsoft.AspNet.Http;

namespace CalDAV.Core.ConditionsCheck.Preconditions.Report
{
    public interface IReportPreconditions
    {
        /// <summary>
        ///     Call this method from the Report and it will
        ///     hadle the preconditions.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns>
        ///     False if any of the preconditions fails.
        ///     True otherwise.
        /// </returns>
        bool PreconditionProcessor(HttpContext httpContext);


        /// <summary>
        ///     (CALDAV:supported-calendar-data): The attributes "content-type" and "version" of the
        ///     CALDAV:calendar-data XML element(see Section 9.6) specify a media type supported by the server for
        ///     calendar object resources
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        bool SuppoprtedCalendarData(string contentType, string version);

        /// <summary>
        ///     (CALDAV:valid-filter): The CALDAV:filter XML element (see Section 9.7) specified in the REPORT r
        ///     request MUST be valid.For instance, a CALDAV:filter cannot nest a
        ///     <C:comp name="VEVENT">
        ///         element in a
        ///         <C:comp name="VTODO">
        ///             element, and a CALDAV:filter cannot nest a
        ///             <C:time-range
        ///                 start="..." end="...">
        ///                 element in a<C:prop name="SUMMARY"> element
        /// </summary>
        /// <param name="xDoc">THe filter element from the body of the request</param>
        /// <returns></returns>
        bool ValidFilter(XDocument xDoc);

        /// <summary>
        ///     (CALDAV:supported-filter): The CALDAV:comp-filter (see Section 9.7.1), CALDAV:prop-filter
        ///     (seeSection 9.7.2), and CALDAV:param-filter(see Section 9.7.3) XML elements used in the CALDAV:filter
        ///     XML element(see Section 9.7) in the REPORT request only make reference to components, properties,
        ///     and parameters for which queries are supported by the server, i.e., if the CALDAV:filter element attempts
        ///     to reference an unsupported component, property, or parameter, this precondition is violated.Servers
        ///     SHOULD report the CALDAV:comp-filter, CALDAV:prop-filter, or CALDAV:param-filter for which it
        ///     does not provide support.
        /// </summary>
        /// <param name="compFilter"></param>
        /// <returns></returns>
        bool SupportedFilter(XDocument compFilter);

        /// <summary>
        ///     (CALDAV:valid-calendar-data): The time zone specified in the REPORT request
        ///     MUST be a valid iCalendar object containing a single valid VTIMEZONE component.
        /// </summary>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        bool ValidCalendarData(VCalendar timeZone);

        /// <summary>
        ///     (CALDAV:min-date-time): Any XML element specifying a range of time MUST have its start or end
        ///     DATE or DATE-TIME values greater than or equal to the value of the CALDAV:min-date-time property
        ///     value(Section 5.2.6) on the calendar collections being targeted by the REPORT request;
        /// </summary>
        /// <param name="dts">A list containing the datetimes that are declared in the filter request.</param>
        /// <returns></returns>
        bool MinDateTime(List<DateTime> dts);

        /// <summary>
        ///     (CALDAV:max-date-time): Any XML element specifying a range of time MUST have its start or end
        ///     DATE or DATE-TIME values less than or equal to the value of the CALDAV:max-date-time property
        ///     value(Section 5.2.7) on the calendar collections being targeted by the REPORT request
        /// </summary>
        /// <param name="dts"></param>
        /// <returns></returns>
        bool MaxDateTime(List<DateTime> dts);

        /// <summary>
        ///     (CALDAV:supported-collation): Any XML attribute specifying a collation MUST
        ///     specify a collationsupported by the server as described in Section 7.5.
        /// </summary>
        /// <param name="collation">The collation value of the request.</param>
        /// <returns></returns>
        bool SupportedCollation(string collation);

        /// <summary>
        ///     Check is the principal has the permission to perform
        ///     an operation on the resource.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        bool HasPermission(HttpContext httpContext);


        /// <summary>
        /// Set the error in the respose.
        /// </summary>
        /// <param name="statusCode">The status code for the response.</param>
        /// <param name="errorMessage">The precondition element that failed.</param>
        /// <param name="httpContext">The httpContext that contains the response.</param>
        void SetErrorResponse(int statusCode, string errorMessage, HttpContext httpContext);
    }
}