using System;
using System.Text;
using CalDAV.Core.Method_Extensions;
using ICalendar.Calendar;
using ICalendar.GeneralInterfaces;
using TreeForXml;
using Xunit;

namespace CalDav_tests
{
    public class FiltersTests
    {
        /// <summary>
        ///     7.8.5 Example: Retrieval of To-Dos by Alarm Time Range
        /// </summary>
        [Fact]
        public void UnitTest3()
        {
            var calStr = @"BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//Example Corp.//CalDAV Client//EN
BEGIN:VTODO
DTSTAMP:20060205T235300Z
DUE;TZID=US/Eastern:20060106T120000
LAST-MODIFIED:20060205T235308Z
SEQUENCE:1
STATUS:NEEDS-ACTION
SUMMARY:Task #2
UID:E10BA47467C5C69BB74E8720@example.com
BEGIN:VALARM
ACTION:AUDIO
TRIGGER;RELATED=START:-PT10M
END:VALARM
END:VTODO
END:VCALENDAR";
            var xmlStr = @"<C:filter  xmlns:C=""urn:ietf:params:xml:ns:caldav"">
<C:comp-filter name=""VCALENDAR"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
<C:comp-filter name=""VTODO"">
<C:comp-filter name=""VALARM"">
<C:time-range start=""20060106T100000Z""
end=""20060107T100000Z""/>
</C:comp-filter>
</C:comp-filter>
</C:comp-filter></C:filter>";
            var calendar = new VCalendar(calStr);
            var xmlTree = XmlTreeStructure.Parse(xmlStr);
            var result = calendar.FilterResource(xmlTree);
            Assert.True(result);
        }

        [Fact]
        public void RecursiveSeekerTest2()
        {
            var calStr = @"BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//Example Corp.//CalDAV Client//EN
BEGIN:VTIMEZONE
LAST-MODIFIED:20040110T032845Z
TZID:US/Eastern
BEGIN:DAYLIGHT
DTSTART:20000404T020000
RRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=4
TZNAME:EDT
TZOFFSETFROM:-0500
TZOFFSETTO:-0400
END:DAYLIGHT
BEGIN:STANDARD
DTSTART:20001026T020000
RRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10
TZNAME:EST
TZOFFSETFROM:-0400
TZOFFSETTO:-0500
END:STANDARD
END:VTIMEZONE
BEGIN:VEVENT
ATTENDEE;PARTSTAT=ACCEPTED;ROLE=CHAIR:mailto:cyrus@example.com
DTSTAMP:20060206T001220Z
DTSTART;TZID=US/Eastern:20060104T100000
DURATION:PT1H
LAST-MODIFIED:20060206T001330Z
ORGANIZER:mailto:cyrus@example.com
SEQUENCE:1
STATUS:TENTATIVE
SUMMARY:Event #3
UID:DC6C50A017428C5216A2F1CD@example.com
X-ABC-GUID:E1CX5Dr-0007ym-Hz@example.com
END:VEVENT
END:VCALENDAR";
            var xmlStr = @"<C:comp-filter name=""VCALENDAR"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
<C:comp-filter name=""VEVENT"">
<C:prop-filter name=""UID"">
<C:text-match collation=""i;octet""
>DC6C50A017428C5216A2F1CD@example.com</C:text-match>
</C:prop-filter>
</C:comp-filter>
</C:comp-filter>";
            var calendar = new VCalendar(calStr);
            var xmlTree = XmlTreeStructure.Parse(xmlStr);
            IXMLTreeStructure tree;
            ICalendarComponent comp;
            var result = calendar.ComponentSeeker(xmlTree, out tree, out comp);
            Assert.True(result);
            Assert.Equal("VEVENT", comp.Name);
            Assert.Equal("VEVENT", tree.Attributes["name"]);
        }

        [Fact]
        public void RecursiveSeekerTest3()
        {
            var calStr = @"BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//Example Corp.//CalDAV Client//EN
BEGIN:VTIMEZONE
LAST-MODIFIED:20040110T032845Z
TZID:US/Eastern
BEGIN:DAYLIGHT
DTSTART:20000404T020000
RRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=4
TZNAME:EDT
TZOFFSETFROM:-0500
TZOFFSETTO:-0400
END:DAYLIGHT
BEGIN:STANDARD
DTSTART:20001026T020000
RRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10
TZNAME:EST
TZOFFSETFROM:-0400
TZOFFSETTO:-0500
END:STANDARD
END:VTIMEZONE
BEGIN:VEVENT
ATTENDEE;PARTSTAT=ACCEPTED;ROLE=CHAIR:mailto:cyrus@example.com
DTSTAMP:20060206T001220Z
DTSTART;TZID=US/Eastern:20060104T100000
DURATION:PT1H
LAST-MODIFIED:20060206T001330Z
ORGANIZER:mailto:cyrus@example.com
SEQUENCE:1
STATUS:TENTATIVE
SUMMARY:Event #3
UID:DC6C50A017428C5216A2F1CD@example.com
X-ABC-GUID:E1CX5Dr-0007ym-Hz@example.com
END:VEVENT
END:VCALENDAR";
            var xmlStr = @"<C:comp-filter name=""VCALENDAR"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
<C:comp-filter name=""VTODO"">
<C:prop-filter name=""UID"">
<C:text-match collation=""i;octet""
>DC6C50A017428C5216A2F1CD@example.com</C:text-match>
</C:prop-filter>
</C:comp-filter>
</C:comp-filter>";
            var calendar = new VCalendar(calStr);
            var xmlTree = XmlTreeStructure.Parse(xmlStr);
            IXMLTreeStructure tree;
            ICalendarComponent comp;
            var result = calendar.ComponentSeeker(xmlTree, out tree, out comp);
            Assert.False(result);
            /*Assert.Equal("VEVENT", comp.Name);
			Assert.Equal("VEVENT", tree.Attributes["name"]);*/
        }


        /// <summary>
        ///     Test for the ApplyTextFilter.
        ///     Should apply a substring filter.
        /// </summary>
        [Fact]
        public void UnitTest4()
        {
            var propValueOctet = Encoding.ASCII.GetBytes("DC6C50A017428C5216A2F1CD@example.comwithsomemore");
            var filterValueOctet = Encoding.ASCII.GetBytes("DC6C50A017428C5216A2F1CD@example.com");
            var result = ExtensionsForFilters.ApplyTextFilter(propValueOctet, filterValueOctet);
            Assert.True(result);
        }


        [Fact]
        public void FilterResourceTest1()
        {
            var calStr = @"BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//Example Corp.//CalDAV Client//EN
BEGIN:VTIMEZONE
LAST-MODIFIED:20040110T032845Z
TZID:US/Eastern
BEGIN:DAYLIGHT
DTSTART:20000404T020000
RRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=4
TZNAME:EDT
TZOFFSETFROM:-0500
TZOFFSETTO:-0400
END:DAYLIGHT
BEGIN:STANDARD
DTSTART:20001026T020000
RRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10
TZNAME:EST
TZOFFSETFROM:-0400
TZOFFSETTO:-0500
END:STANDARD
END:VTIMEZONE
BEGIN:VEVENT
ATTENDEE;PARTSTAT=NEEDS-ACTION:mailto:lisa@example.com
DTSTAMP:20060206T001220Z
DTSTART;TZID=US/Eastern:20060104T100000
DURATION:PT1H
LAST-MODIFIED:20060206T001330Z
ORGANIZER:mailto:cyrus@example.com
SEQUENCE:1
STATUS:TENTATIVE
SUMMARY:Event #3
UID:DC6C50A017428C5216A2F1CD@example.com
X-ABC-GUID:E1CX5Dr-0007ym-Hz@example.com
END:VEVENT
END:VCALENDAR";
            var xmlStr = @"<C:filter  xmlns:C=""urn:ietf:params:xml:ns:caldav"">
<C:comp-filter name=""VCALENDAR"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
	<C:comp-filter name=""VEVENT"">
		<C:prop-filter name=""ATTENDEE"">
			<C:text-match collation=""i;ascii-casemap"">mailto:lisa@example.com</C:text-match>
			<C:param-filter name=""PARTSTAT"">
				 <C:text-match collation=""i;ascii-casemap"">NEEDS-ACTION</C:text-match>
			</C:param-filter>
		</C:prop-filter>
	</C:comp-filter>
</C:comp-filter></C:filter>";
            var calendar = new VCalendar(calStr);
            var xmlTree = XmlTreeStructure.Parse(xmlStr);
            var result = calendar.FilterResource(xmlTree);
            Assert.True(result);
        }


        [Fact]
        public void FilterResourceTest2()
        {
            var calStr = @"BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//Example Corp.//CalDAV Client//EN
BEGIN:VTIMEZONE
LAST-MODIFIED:20040110T032845Z
TZID:US/Eastern
BEGIN:DAYLIGHT
DTSTART:20000404T020000
RRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=4
TZNAME:EDT
TZOFFSETFROM:-0500
TZOFFSETTO:-0400
END:DAYLIGHT
BEGIN:STANDARD
DTSTART:20001026T020000
RRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10
TZNAME:EST
TZOFFSETFROM:-0400
TZOFFSETTO:-0500
END:STANDARD
END:VTIMEZONE
BEGIN:VEVENT
ATTENDEE;PARTSTAT=NEEDS-ACTION:mailto:lisa@example.com
DTSTAMP:20060206T001220Z
DTSTART;TZID=US/Eastern:20060104T100000
DURATION:PT1H
LAST-MODIFIED:20060206T001330Z
ORGANIZER:mailto:cyrus@example.com
SEQUENCE:1
STATUS:TENTATIVE
SUMMARY:Event #3
UID:DC6C50A017428C5216A2F1CD@example.com
X-ABC-GUID:E1CX5Dr-0007ym-Hz@example.com
END:VEVENT
END:VCALENDAR";
            var xmlStr = @"<C:comp-filter name=""VCALENDAR"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
	<C:comp-filter name=""VEVENT"">
<C:prop-filter name=""UID"">
<C:text-match collation=""i;octet""
>DC6C50A017428C5216A2F1CD@example.com</C:text-match>
</C:prop-filter>
</C:comp-filter>
</C:comp-filter>";
            var calendar = new VCalendar(calStr);
            var xmlTree = XmlTreeStructure.Parse(xmlStr);
            var result = calendar.FilterResource(xmlTree);
            Assert.True(result);
        }


        [Fact]
        public void RetrievalofAllPendingToDos()
        {
            var xmlStr = @"<C:filter  xmlns:C=""urn:ietf:params:xml:ns:caldav"">
<C:comp-filter name=""VCALENDAR"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
	<C:comp-filter name=""VTODO"">
<C:prop-filter name=""COMPLETED"">
<C:is-not-defined/>
</C:prop-filter>
<C:prop-filter name=""STATUS"">
<C:text-match
negate-condition=""yes"">CANCELLED</C:text-match>
</C:prop-filter>
</C:comp-filter>
</C:comp-filter></C:filter>";
            var calStr = @"BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//Example Corp.//CalDAV Client//EN
BEGIN:VTODO
DTSTAMP:20060205T235335Z
DUE;VALUE=DATE:20060104
STATUS:NEEDS-ACTION
SUMMARY:Task #1
UID:DDDEEB7915FA61233B861457@example.com
BEGIN:VALARM
ACTION:AUDIO
TRIGGER;RELATED=START:-PT10M
END:VALARM
END:VTODO
END:VCALENDAR";
            var calendar = new VCalendar(calStr);
            var xmlTree = XmlTreeStructure.Parse(xmlStr);
            var result = calendar.FilterResource(xmlTree);
            Assert.True(result);
        }

        [Fact]
        public void PartialRetrievalofEventsbyTimeRange()
        {
            var xmlStr = @"<C:filter  xmlns:C=""urn:ietf:params:xml:ns:caldav"">
<C:comp-filter name=""VCALENDAR"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
	<C:comp-filter name=""VEVENT"">
		<C:time-range start=""20060104T000000Z"" end=""20060105T000000Z""/>
	</C:comp-filter>
</C:comp-filter></C:filter>";
            var calStr = @"BEGIN:VCALENDAR
VERSION:2.0
BEGIN:VTIMEZONE
LAST-MODIFIED:20040110T032845Z
TZID:US/Eastern
BEGIN:DAYLIGHT
DTSTART:20000404T020000
RRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=4
TZNAME:EDT
TZOFFSETFROM:-0500
TZOFFSETTO:-0400
END:DAYLIGHT
BEGIN:STANDARD
DTSTART:20001026T020000
RRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10
TZNAME:EST
TZOFFSETFROM:-0400
TZOFFSETTO:-0500
END:STANDARD
END:VTIMEZONE
BEGIN:VEVENT
DTSTART;TZID=US/Eastern:20060102T120000
DURATION:PT1H
RRULE:FREQ=DAILY;COUNT=5
SUMMARY:Event #2
UID:00959BC664CA650E933C892C@example.com
END:VEVENT
END:VCALENDAR";
            var calendar = new VCalendar(calStr);
            var xmlTree = XmlTreeStructure.Parse(xmlStr);
            var result = calendar.FilterResource(xmlTree);
            Assert.True(result);
        }

        /// <summary>
        ///     7.8.1 Example: Partial Retrieval of Events by Time Range
        /// </summary>
        [Fact]
        public void UnitTest1()
        {
            var xmlStr = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<C:calendar-query xmlns:D=""DAV:""
xmlns:C=""urn:ietf:params:xml:ns:caldav"">
<D:prop>
<D:getetag/>
<C:calendar-data>
<C:comp name=""VCALENDAR"">
<C:prop name=""VERSION""/>
<C:comp name=""VEVENT"">
<C:prop name=""SUMMARY""/>
<C:prop name=""UID""/>
<C:prop name=""DTSTART""/>
<C:prop name=""DTEND""/>
<C:prop name=""DURATION""/>
<C:prop name=""RRULE""/>
<C:prop name=""RDATE""/>
<C:prop name=""EXRULE""/>
<C:prop name=""EXDATE""/>
<C:prop name=""RECURRENCE-ID""/>
</C:comp>
<C:comp name=""VTIMEZONE""/>
</C:comp>
</C:calendar-data>
</D:prop>
<C:filter>
<C:comp-filter name=""VCALENDAR"">
<C:comp-filter name=""VEVENT"">
<C:time-range start=""20060104T000000Z""
end=""20060105T000000Z""/>
</C:comp-filter>
</C:comp-filter>
</C:filter>
</C:calendar-query>";
            var calStr = @"BEGIN:VCALENDAR
VERSION:2.0
BEGIN:VTIMEZONE
LAST-MODIFIED:20040110T032845Z
TZID:US/Eastern
BEGIN:DAYLIGHT
DTSTART:20000404T020000
RRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=4
TZNAME:EDT
TZOFFSETFROM:-0500
TZOFFSETTO:-0400
END:DAYLIGHT
BEGIN:STANDARD
DTSTART:20001026T020000
RRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10
TZNAME:EST
TZOFFSETFROM:-0400
TZOFFSETTO:-0500
END:STANDARD
END:VTIMEZONE
BEGIN:VEVENT
DTSTART;TZID=US/Eastern:20060102T120000
DURATION:PT1H
RRULE:FREQ=DAILY;COUNT=5
SUMMARY:Event #2
UID:00959BC664CA650E933C892C@example.com
END:VEVENT
BEGIN:VEVENT
DTSTART;TZID=US/Eastern:20060104T140000
DURATION:PT1H
RECURRENCE-ID;TZID=US/Eastern:20060104T120000
SUMMARY:Event #2 bis
UID:00959BC664CA650E933C892C@example.com
END:VEVENT
BEGIN:VEVENT
DTSTART;TZID=US/Eastern:20060106T140000
DURATION:PT1H
RECURRENCE-ID;TZID=US/Eastern:20060106T120000
SUMMARY:Event #2 bis bis
UID:00959BC664CA650E933C892C@example.com
END:VEVENT
END:VCALENDAR";
            var calendar = new VCalendar(calStr);
            var xmlTree = XmlTreeStructure.Parse(xmlStr);
            var result = calendar.FilterResource(xmlTree);
            Assert.True(result);
        }

        /// <summary>
        ///     7.8.4 Example: Partial Retrieval of Stored Free Busy Components
        /// </summary>
        [Fact]
        public void UnitTest2()
        {
            var xmlStr = @"<C:filter xmlns:C=""urn:ietf:params:xml:ns:caldav"">
                   <C:comp-filter name=""VCALENDAR"">
                <C:comp-filter name=""VFREEBUSY"">
                <C:time-range start=""20060102T000000Z""
                end=""20060103T000000Z""/>
                </C:comp-filter>
                </C:comp-filter>
                </C:filter>";
            var calStr = @"BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//Example Corp.//CalDAV Client//EN
BEGIN:VFREEBUSY
ORGANIZER;CN=""Bernard Desruisseaux"":mailto:bernard@example.com
UID:76ef34-54a3d2@example.com
DTSTAMP:20050530T123421Z
DTSTART:20060101T100000Z
DTEND:20060108T100000Z
FREEBUSY;FBTYPE=BUSY-TENTATIVE:20060102T100000Z/20060102T120000Z
END:VFREEBUSY
END:VCALENDAR";

            var calendar = VCalendar.Parse(calStr);
            var xmlTree = XmlTreeStructure.Parse(xmlStr);
            var result = calendar.FilterResource(xmlTree);
            Assert.True(result);
        }


        [Fact]
        public void TestingStuffs()
        {
            var temp = TimeZoneInfo.GetSystemTimeZones();
        }
    }
}