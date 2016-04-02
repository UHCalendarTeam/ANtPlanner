using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.Core;
using TreeForXml;
using Xunit;

namespace CalDav_tests.Report
{
    public class REPORT_Tests
    {
        /// <summary>
        /// 7.8.1 Example: Partial Retrieval of Events by Time Range
        /// </summary>
        [Fact]
        public void UnitTest1()
        {
            IFileSystemManagement fs = new FileSystemManagement("bernard", "work");
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

            var xmlTree = XmlTreeStructure.Parse(xmlStr);

            var reportMet=new ReportMethods();

            var result = reportMet.ProcessRequest(xmlTree, fs);
        }
    }
}
