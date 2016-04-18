using System.Linq;
using System.Xml.Linq;
using CalDAV.Core;
using DataLayer;
using TreeForXml;
using Xunit;

namespace CalDav_tests.Report
{
    public class REPORT_Tests
    {
        /// <summary>
        ///     7.8.1 Example: Partial Retrieval of Events by Time Range
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

            var reportMet = new CalDavReport();

            var result = reportMet.ProcessRequest(xmlTree, fs);


            var xmlResult = XDocument.Parse(result);

            XNamespace nsDAV = "DAV:";


            var responses = xmlResult.Root.Elements(nsDAV + "response");

            Assert.Equal(2, responses.Count());

            var hrefs = responses.SelectMany(x => x.Elements(nsDAV + "href"));

            Assert.Equal(2, hrefs.Count());

            Assert.True(hrefs.First().Value.Contains("abcd2.ics"));
            Assert.True(hrefs.Last().Value.Contains("abcd3.ics"));
        }


        /// <summary>
        ///     7.8.7 Example: Retrieval of Events by PARTSTAT
        /// </summary>
        [Fact]
        public void UnitTest2()
        {
            var xmlStr = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<C:calendar-query xmlns:C=""urn:ietf:params:xml:ns:caldav"">
<D:prop xmlns:D=""DAV:"">
<D:getetag/>
<C:calendar-data/>
</D:prop>
<C:filter>
<C:comp-filter name=""VCALENDAR"">
<C:comp-filter name=""VEVENT"">
<C:prop-filter name=""ATTENDEE"">
<C:text-match collation=""i;ascii-casemap""
>mailto:lisa@example.com</C:text-match>
<C:param-filter name=""PARTSTAT"">
<C:text-match collation=""i;ascii-casemap""
>NEEDS-ACTION</C:text-match>
</C:param-filter>
</C:prop-filter>
</C:comp-filter>
</C:comp-filter>
</C:filter>
</C:calendar-query>";

            IFileSystemManagement fs = new FileSystemManagement("bernard", "work");

            var xmlTree = XmlTreeStructure.Parse(xmlStr);

            var reportMet = new CalDavReport();

            var result = reportMet.ProcessRequest(xmlTree, fs);


            var xmlResult = XDocument.Parse(result);

            XNamespace nsDAV = "DAV:";


            var responses = xmlResult.Root.Elements(nsDAV + "response");

            Assert.Equal(1, responses.Count());

            var hrefs = responses.SelectMany(x => x.Elements(nsDAV + "href"));

            Assert.Equal(1, hrefs.Count());

            Assert.True(hrefs.First().Value.Contains("abcd3.ics"));
        }

        /// <summary>
        ///     7.8.8 Example: Retrieval of Events Only
        /// </summary>
        [Fact]
        public void UnitTest3()
        {
            var xmlStr = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<C:calendar-query xmlns:C=""urn:ietf:params:xml:ns:caldav"">
<D:prop xmlns:D=""DAV:"">
<D:getetag/>
<C:calendar-data/>
</D:prop>
<C:filter>
<C:comp-filter name=""VCALENDAR"">
<C:comp-filter name=""VEVENT""/>
</C:comp-filter>
</C:filter>
</C:calendar-query>";

            IFileSystemManagement fs = new FileSystemManagement("bernard", "work");

            var xmlTree = XmlTreeStructure.Parse(xmlStr);

            var reportMet = new CalDavReport();

            var result = reportMet.ProcessRequest(xmlTree, fs);


            var xmlResult = XDocument.Parse(result);

            XNamespace nsDAV = "DAV:";


            var responses = xmlResult.Root.Elements(nsDAV + "response");

            Assert.Equal(3, responses.Count());

            var hrefs = responses.SelectMany(x => x.Elements(nsDAV + "href"));

            Assert.Equal(3, hrefs.Count());

            var hrefValues = hrefs.Select(x => x.Value).ToList();

            Assert.EndsWith("abcd1.ics", hrefValues[0]);
            Assert.EndsWith("abcd2.ics", hrefValues[1]);
            Assert.EndsWith("abcd3.ics", hrefValues[2]);
        }

        /// <summary>
        ///     7.8.9 Example: Retrieval of All Pending To-Dos
        /// </summary>
        [Fact]
        public void UnitTest4()
        {
            var xmlStr = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<C:calendar-query xmlns:C=""urn:ietf:params:xml:ns:caldav"">
<D:prop xmlns:D=""DAV:"">
<D:getetag/>
<C:calendar-data/>
</D:prop>
<C:filter>
<C:comp-filter name=""VCALENDAR"">
<C:comp-filter name=""VTODO"">
<C:prop-filter name=""COMPLETED"">
<C:is-not-defined/>
</C:prop-filter>
<C:prop-filter name=""STATUS"">
<C:text-match
negate-condition=""yes"">CANCELLED</C:text-match>
</C:prop-filter>
</C:comp-filter>
</C:comp-filter>
</C:filter>
</C:calendar-query>";


            IFileSystemManagement fs = new FileSystemManagement("bernard", "work");

            var xmlTree = XmlTreeStructure.Parse(xmlStr);

            var reportMet = new CalDavReport();

            var result = reportMet.ProcessRequest(xmlTree, fs);


            var xmlResult = XDocument.Parse(result);

            XNamespace nsDAV = "DAV:";
            XNamespace nsCalDAV = "urn:ietf:params:xml:ns:caldav";


            var calDatas = xmlResult.Root.Descendants(nsCalDAV + "calendar-data");

            Assert.Equal(2, calDatas.Count());


            var responses = xmlResult.Root.Elements(nsDAV + "response");

            Assert.Equal(2, responses.Count());

            var hrefs = responses.SelectMany(x => x.Elements(nsDAV + "href"));

            Assert.Equal(2, hrefs.Count());

            var hrefValues = hrefs.Select(x => x.Value).ToList();

            Assert.EndsWith("abcd4.ics", hrefValues[0]);
            Assert.EndsWith("abcd5.ics", hrefValues[1]);
        }


        [Fact]
        public void UnitTest5()
        {
            var xmlStr = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<C:calendar-multiget xmlns:D=""DAV:""
xmlns:C=""urn:ietf:params:xml:ns:caldav"">
<D:prop>
<D:getetag/>
<C:calendar-data/>
</D:prop>
<D:href>/bernard/work/abcd1.ics</D:href>
<D:href>/bernard/work/mtg1.ics</D:href>
</C:calendar-multiget>";


            IFileSystemManagement fs = new FileSystemManagement("bernard", "work");

            var xmlTree = XmlTreeStructure.Parse(xmlStr);

            var reportMet = new CalDavReport();

            var result = reportMet.ProcessRequest(xmlTree, fs);


            var xmlResult = XDocument.Parse(result);

            XNamespace nsDAV = "DAV:";
            XNamespace nsCalDAV = "urn:ietf:params:xml:ns:caldav";

            var status = xmlResult.Root.Descendants(nsDAV + "status");

            Assert.Equal(2, status.Count());

            Assert.Equal("HTTP/1.1 200 OK", status.First().Value);

            Assert.Equal("HTTP/1.1 404 Not Found", status.Last().Value);
        }
    }
}