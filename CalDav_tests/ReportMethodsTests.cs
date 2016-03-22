using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.Models.Method_Extensions;
using ICalendar.Calendar;
using ICalendar.GeneralInterfaces;
using TreeForXml;
using Xunit;

namespace CalDav_tests
{
    public class ReportMethodsTests
    {
        [Fact]
        public void RecursiveSeekerTest()
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
            var xmlStr = @"<C:comp-filter name=""VCALENDAR"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
<C:comp-filter name=""VTODO"">
<C:comp-filter name=""VALARM"">
<C:time-range start=""20060106T100000Z""
end=""20060107T100000Z""/>
</C:comp-filter>
</C:comp-filter>
</C:comp-filter>";
            var calendar = new VCalendar(calStr);
            var xmlTree = XmlTreeStructure.Parse(xmlStr);
            var dict = new Dictionary<string, VCalendar>() {
                {"VCALENDAR",calendar}
            };
            IXMLTreeStructure tree;
            ICalendarComponent comp;
            Extensions.RecursiveSeeker(calendar, xmlTree, out tree, out comp);
            Assert.Equal("VALARM", comp.Name);
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
            Extensions.RecursiveSeeker(calendar, xmlTree, out tree, out comp);
            Assert.Equal("VEVENT", comp.Name);
            Assert.Equal("VEVENT", tree.Attributes["name"]);
        }
    }
}
