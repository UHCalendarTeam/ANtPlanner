using System.Collections.Generic;
using ICalendar.Calendar;
using TreeForXml;

namespace CalDAV.Core
{
    internal interface IReportMethods
    {
        string ExpandProperty();
        string CalendarQuery(IXMLTreeStructure filters);

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
        string ToXmlString(IEnumerable<KeyValuePair<string, VCalendar>> resources,
            IXMLTreeStructure calendarData);
    }
}