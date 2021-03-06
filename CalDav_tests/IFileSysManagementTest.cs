﻿//using DataLayer;
//using Xunit;

//namespace CalDav_tests
//{
//    public class IFileSysManagementTest
//    {
//        /// <summary>
//        ///     Create a folder.
//        /// </summary>
//        [Fact]
//        public void UnitTest1()
//        {
//            IFileManagement fs = new FileManagement("foo", "calendar1");

//            Assert.True(fs.AddUserFolder("foo"));

//            Assert.True(fs.CreateFolder(TODO));

//            Assert.True(fs.ExistCalendarCollection(TODO));

//            Assert.True(fs.DeleteCalendarCollection());

//            Assert.False(fs.ExistCalendarCollection(TODO));
//        }

//        /// <summary>
//        ///     Adding and deleting resources
//        /// </summary>
//        [Fact]
//        public void UnitTest2()
//        {
//            var calStr = @"BEGIN:VCALENDAR
//VERSION:2.0
//BEGIN:VTIMEZONE
//LAST-MODIFIED:20040110T032845Z
//TZID:US/Eastern
//BEGIN:DAYLIGHT
//DTSTART:20000404T020000
//RRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=4
//TZNAME:EDT
//TZOFFSETFROM:-0500
//TZOFFSETTO:-0400
//END:DAYLIGHT
//BEGIN:STANDARD
//DTSTART:20001026T020000
//RRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10
//TZNAME:EST
//TZOFFSETFROM:-0400
//TZOFFSETTO:-0500
//END:STANDARD
//END:VTIMEZONE
//BEGIN:VEVENT
//DTSTART;TZID=US/Eastern:20060102T120000
//DURATION:PT1H
//RRULE:FREQ=DAILY;COUNT=5
//SUMMARY:Event #2
//UID:00959BC664CA650E933C892C@example.com
//END:VEVENT
//BEGIN:VEVENT
//DTSTART;TZID=US/Eastern:20060104T140000
//DURATION:PT1H
//RECURRENCE-ID;TZID=US/Eastern:20060104T120000
//SUMMARY:This event has more that 75 chars, so the line has to be splitted. Testing is split ok.
//UID:00959BC664CA650E933C892C@example.com
//END:VEVENT
//BEGIN:VEVENT
//DTSTART;TZID=US/Eastern:20060106T140000
//DURATION:PT1H
//RECURRENCE-ID;TZID=US/Eastern:20060106T120000
//SUMMARY:Event #2 bis bis
//UID:00959BC664CA650E933C892C@example.com
//END:VEVENT
//END:VCALENDAR";

//            IFileManagement fs = new FileManagement("foo", "calendar1");

//            Assert.True(fs.AddUserFolder("foo"));

//            Assert.True(fs.CreateFolder(TODO));

//            Assert.True(fs.ExistCalendarCollection(TODO));

//            Assert.True(fs.AddCalendarObjectResourceFile(TODO, TODO).Result);

//            Assert.True(fs.ExistCalendarObjectResource("resource1.ics"));

//            var fileStr = fs.GetCalendarObjectResource("resource1.ics");
//        }
//    }
//}

