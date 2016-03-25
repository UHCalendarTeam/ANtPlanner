using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.Models.Method_Extensions;
using ICalendar.Utils;
using ICalendar.ValueTypes;
using Xunit;

namespace CalDav_tests
{
    public class RULESTests
    {
        [Fact]
        public void DaylyFor10Ocurrences()
        {
            DateTime? startTime ;
            "19970902T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur = new Recur
            {
                Frequency = RecurValues.Frequencies.DAILY,
                Count = 10
            };
            recurs.Add(recur);
            var dts = startTime.Value.ExpandTime(recurs);

            Assert.Equal(10, dts.Count());

        }

        [Fact]
        public void DaylyUntil24121997()
        {
            DateTime? startTime ;
            "19970902T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=DAILY;UNTIL=19971224T000000Z".ToRecur(out recur);
            recur.Frequency = RecurValues.Frequencies.DAILY;
            recurs.Add(recur);
            var dts = startTime.Value.ExpandTime(recurs);
            var endDt = startTime.Value.AddMonths(3).AddDays(21);
            Assert.Equal(endDt, dts.Last());

        }

        [Fact]
        public void EveryOtherDayForever()
        {
            DateTime? startTime ;
            "19970902T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=DAILY;INTERVAL=2".ToRecur(out recur);
            recur.Frequency = RecurValues.Frequencies.DAILY;
            recurs.Add(recur);
            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(1000,dts.Count() );

        }

        [Fact]
        public void Every10Days()
        {
            DateTime? startTime ;
            "19970902T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=DAILY;INTERVAL=10;COUNT=5".ToRecur(out recur);
            recur.Frequency = RecurValues.Frequencies.DAILY;
            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>(5) {startTime.Value};
            for (int i = 0; i < 4; i++)
            {
                expected.Add(expected.Last().AddDays(10));
            }
            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(5, dts.Count());
            foreach (var dt in expected)
            {
                  Assert.Contains(dt,dts );
            }
          

        }

        /// <summary>
        /// Every day in January, for 3 years:
        /// </summary>
        [Fact]
        public void UnitTest5()
        {
            DateTime? startTime ;
            "19980101T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=YEARLY;UNTIL=20000131T140000Z;BYMONTH=1;BYDAY=SU,MO,TU,WE,TH,FR,SA".ToRecur(out recur);
           recur.Frequency = RecurValues.Frequencies.YEARLY;
            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>() {startTime.Value};
            for (int year = 1; year < 3; year++)
            {
                  for (int i = 1; i < 31; i++)
                    {
                    expected.Add(expected.Last().AddDays(1));
                    }
                  expected.Add(expected.First().AddYears(year));
            }
          
            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(93, dts.Count());
            foreach (var dt in expected)
            {
                  Assert.Contains(dt,dts );
            }
          

        }
 /// <summary>
        /// Every day in January, for 3 years:
        /// </summary>
        [Fact]
        public void UnitTest6()
        {
            DateTime? startTime ;
            "19980101T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=DAILY;UNTIL=20000131T140000Z;BYMONTH=1".ToRecur(out recur);
           recur.Frequency = RecurValues.Frequencies.DAILY;
            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>() {startTime.Value};
            for (int year = 1; year < 3; year++)
            {
                  for (int i = 1; i < 31; i++)
                    {
                    expected.Add(expected.Last().AddDays(1));
                    }
                  expected.Add(expected.First().AddYears(year));
            }
          
            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(93, dts.Count());
            foreach (var dt in expected)
            {
                  Assert.Contains(dt,dts );
            }
          

        }
        /// <summary>
        ///Weekly for 10 occurrences
        /// </summary>
        [Fact]
        public void UnitTest7()
        {
            DateTime? startTime ;
            "19970902T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=WEEKLY;COUNT=10".ToRecur(out recur);
           recur.Frequency = RecurValues.Frequencies.WEEKLY;
            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>() {startTime.Value};
           
            for (int i = 1; i < 10; i++)
            {
                expected.Add(expected.Last().AddDays(7));
            }
                  
            
          
            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(10, dts.Count());
            foreach (var dt in expected)
            {
                  Assert.Contains(dt,dts );
            }
          

        }

        /// <summary>
        ///Weekly until December 24, 1997
        /// </summary>
        [Fact]
        public void UnitTest8()
        {
            DateTime? startTime ;
            "19970902T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=WEEKLY;UNTIL=19971224T000000Z".ToRecur(out recur);
           recur.Frequency = RecurValues.Frequencies.WEEKLY;
            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>() {startTime.Value};
           
            for (int i = 1; i < 17; i++)
            {
                expected.Add(expected.Last().AddDays(7));
            }
                  
            
          
            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(17, dts.Count());
            foreach (var dt in expected)
            {
                  Assert.Contains(dt,dts );
            }
          

        }


        /// <summary>
        ///Weekly on Tuesday and Thursday for five weeks:
        /// </summary>
        [Fact]
        public void UnitTest9()
        {
            DateTime? startTime;
            "19970902T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=WEEKLY;UNTIL=19971007T000000Z;WKST=SU;BYDAY=TU,TH".ToRecur(out recur);
            recur.Frequency = RecurValues.Frequencies.WEEKLY;
            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>() { startTime.Value };

            for (int i = 1; i < 10; i++)
            {
                expected.Add(i % 2 == 0 ? expected.Last().AddDays(5) : expected.Last().AddDays(2));
            }



            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(10, dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }


        }


    }
}
