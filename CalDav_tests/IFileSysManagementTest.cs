using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.Core;
using Xunit;

namespace CalDav_tests
{
    public class IFileSysManagementTest
    {
        /// <summary>
        /// Create a folder.
        /// </summary>
        [Fact]
        public void UnitTest1()
        {
            IFileSystemManagement fs = new FileSystemManagement("foo", "calendar1");

            Assert.True(fs.AddUserFolder("foo"));

            Assert.True(fs.AddCalendarCollectionFolder("foo", "calendar1"));

            Assert.True(fs.ExistCalendarCollection());

            Assert.True(fs.DeleteCalendarCollection());

            Assert.False(fs.ExistCalendarCollection());


        }

        /// <summary>
        /// Adding and deleting resources
        /// </summary>
        [Fact]
        public void UnitTest2()
        {
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

            IFileSystemManagement fs = new FileSystemManagement("foo", "calendar1");

            Assert.True(fs.AddUserFolder("foo"));

            Assert.True(fs.AddCalendarCollectionFolder("foo", "calendar1"));

            Assert.True(fs.ExistCalendarCollection());

            Assert.True(fs.AddCalendarObjectResourceFile("resource1", calStr));

            Assert.True(fs.ExistCalendarObjectResource("resource1.ics"));

            var fileStr = fs.GetCalendarObjectResource("resource1.ics");

            

        }
    }
}
