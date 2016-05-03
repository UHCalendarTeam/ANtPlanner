using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CalDAV.Core.Method_Extensions;
using ICalendar.Utils;
using ICalendar.ValueTypes;
using Xunit;

namespace CalDav_tests
{
    public class RULESTests
    {
        /// <summary>
        ///     Daily for 10 occurrences:
        /// </summary>
        [Fact]
        public void UnitTest1()
        {
            DateTime? startTime;
            "19970902T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=DAILY;COUNT=10".ToRecur(out recur);
            recurs.Add(recur);
            var dts = startTime.Value.ExpandTime(recurs);

            Assert.Equal(10, dts.Count());
        }

        [Fact]
        public void DaylyUntil24121997()
        {
            DateTime? startTime;
            "19970902T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "FREQ=DAILY;UNTIL=19971224T000000Z".ToRecur(out recur);

            recurs.Add(recur);
            var dts = startTime.Value.ExpandTime(recurs);
            var endDt = startTime.Value.AddMonths(3).AddDays(21);
            Assert.Equal(endDt, dts.Last());
        }

        [Fact]
        public void EveryOtherDayForever()
        {
            DateTime? startTime;
            "19970902T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "FREQ=DAILY;INTERVAL=2".ToRecur(out recur);

            recurs.Add(recur);
            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(1000, dts.Count());
        }

        [Fact]
        public void Every10Days()
        {
            DateTime? startTime;
            "19970902T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "FREQ=DAILY;INTERVAL=10;COUNT=5".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>(5) {startTime.Value};
            for (var i = 0; i < 4; i++)
            {
                expected.Add(expected.Last().AddDays(10));
            }
            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(5, dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }

        /// <summary>
        ///     Every day in January, for 3 years:
        /// </summary>
        [Fact]
        public void UnitTest5()
        {
            DateTime? startTime;
            "19980101T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "FREQ=YEARLY;UNTIL=20000131T140000Z;BYMONTH=1;BYDAY=SU,MO,TU,WE,TH,FR,SA".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime> {startTime.Value};
            for (var year = 1; year < 3; year++)
            {
                for (var i = 1; i < 31; i++)
                {
                    expected.Add(expected.Last().AddDays(1));
                }
                expected.Add(expected.First().AddYears(year));
            }

            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(93, dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }

        /// <summary>
        ///     Every day in January, for 3 years:
        /// </summary>
        [Fact]
        public void UnitTest6()
        {
            DateTime? startTime;
            "19980101T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "FREQ=DAILY;UNTIL=20000131T140000Z;BYMONTH=1".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime> {startTime.Value};
            for (var year = 1; year < 3; year++)
            {
                for (var i = 1; i < 31; i++)
                {
                    expected.Add(expected.Last().AddDays(1));
                }
                expected.Add(expected.First().AddYears(year));
            }

            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(93, dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }

        /// <summary>
        ///     Weekly for 10 occurrences
        /// </summary>
        [Fact]
        public void UnitTest7()
        {
            DateTime? startTime;
            "19970902T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "FREQ=WEEKLY;COUNT=10".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime> {startTime.Value};

            for (var i = 1; i < 10; i++)
            {
                expected.Add(expected.Last().AddDays(7));
            }


            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(10, dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }

        /// <summary>
        ///     Weekly until December 24, 1997
        /// </summary>
        [Fact]
        public void UnitTest8()
        {
            DateTime? startTime;
            "19970902T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "FREQ=WEEKLY;UNTIL=19971224T000000Z".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime> {startTime.Value};

            for (var i = 1; i < 17; i++)
            {
                expected.Add(expected.Last().AddDays(7));
            }


            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(17, dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }


        /// <summary>
        ///     Weekly on Tuesday and Thursday for five weeks:
        /// </summary>
        [Fact]
        public void UnitTest9()
        {
            DateTime? startTime;
            "19970902T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "FREQ=WEEKLY;UNTIL=19971007T000000Z;WKST=SU;BYDAY=TU,TH".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime> {startTime.Value};

            for (var i = 1; i < 10; i++)
            {
                expected.Add(i%2 == 0 ? expected.Last().AddDays(5) : expected.Last().AddDays(2));
            }


            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(10, dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }

            "FREQ=WEEKLY;COUNT=10;WKST=SU;BYDAY=TU,TH".ToRecur(out recur);

            recurs = new List<Recur> {recur};
            dts = startTime.Value.ExpandTime(recurs).ToList();
            Assert.Equal(10, dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }


        //  24, 1997, starting on Monday, September 1, 1997:
        /// <summary>
        ///     Every other week on Monday, Wednesday, and Friday until December
        /// </summary>
        [Fact]
        public void UnitTest10()
        {
            DateTime? startTime;
            "19970901T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "FREQ=WEEKLY;INTERVAL=2;UNTIL=19971224T000000Z;WKST=SU;BYDAY=MO,WE,FR".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime> {startTime.Value};

            for (var i = 1; i < 25; i++)
            {
                expected.Add(i%3 == 0 ? expected.Last().AddDays(10) : expected.Last().AddDays(2));
            }


            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(25, dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }

        //  Every other week on Tuesday and Thursday, for 8 occurrences:
        /// <summary>
        ///     Every other week on Monday, Wednesday, and Friday until December
        /// </summary>
        [Fact]
        public void UnitTest11()
        {
            DateTime? startTime;
            "19970902T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "FREQ=WEEKLY;INTERVAL=2;COUNT=8;WKST=SU;BYDAY=TU,TH".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime> {startTime.Value};

            for (var i = 1; i < 8; i++)
            {
                expected.Add(i%2 == 0 ? expected.Last().AddDays(12) : expected.Last().AddDays(2));
            }


            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(8, dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }

        /// <summary>
        ///     Monthly on the first Friday until December 24, 1997:
        /// </summary>
        [Fact]
        public void UnitTest12()
        {
            DateTime? startTime;
            "19970905T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "FREQ=MONTHLY;UNTIL=19971224T000000Z;BYDAY=1FR".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>
            {
                startTime.Value
            };
            DateTime? otherDT;
            "19971003T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19971107T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19971205T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);


            /*  for (int i = 1; i < 8; i++)
              {
                  expected.Add(i % 2 == 0 ? expected.Last().AddDays(12) : expected.Last().AddDays(2));
              }*/


            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }

        /// <summary>
        ///     Every other month on the first and last Sunday of the month for 10
        ///     occurrences:
        /// </summary>
        [Fact]
        public void UnitTest13()
        {
            DateTime? startTime;
            "19970907T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "FREQ=MONTHLY;INTERVAL=2;COUNT=10;BYDAY=1SU,-1SU".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>
            {
                startTime.Value
            };
            DateTime? otherDT;
            "19970928T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);

            "19971102T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19971130T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980104T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980125T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980301T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980329T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);

            "19980503T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980531T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);


            /*  for (int i = 1; i < 8; i++)
              {
                  expected.Add(i % 2 == 0 ? expected.Last().AddDays(12) : expected.Last().AddDays(2));
              }*/


            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }


        /// <summary>
        ///     Monthly on the second-to-last Monday of the month for 6 months:
        ///     occurrences:
        /// </summary>
        [Fact]
        public void UnitTest14()
        {
            DateTime? startTime;
            "19970922T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=MONTHLY;COUNT=6;BYDAY=-2MO".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>
            {
                startTime.Value
            };
            DateTime? otherDT;
            "19971020T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19971117T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19971222T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980119T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980216T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);


            /*  for (int i = 1; i < 8; i++)
              {
                  expected.Add(i % 2 == 0 ? expected.Last().AddDays(12) : expected.Last().AddDays(2));
              }*/


            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }


        /// <summary>
        ///     Monthly on the third-to-the-last day of the month, forever:
        ///     occurrences:
        /// </summary>
        [Fact]
        public void UnitTest15()
        {
            DateTime? startTime;
            "19970928T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=MONTHLY;BYMONTHDAY=-3".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>
            {
                startTime.Value
            };
            DateTime? otherDT;

            "19971029T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19971128T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19971229T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980129T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980226T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);


            /*  for (int i = 1; i < 8; i++)
              {
                  expected.Add(i % 2 == 0 ? expected.Last().AddDays(12) : expected.Last().AddDays(2));
              }*/


            var dts = startTime.Value.ExpandTime(recurs).Take(expected.Count());
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }

        /// <summary>
        ///     Monthly on the 2nd and 15th of the month for 10 occurrences:
        ///     occurrences:
        /// </summary>
        [Fact]
        public void UnitTest16()
        {
            DateTime? startTime;
            "19970902T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=MONTHLY;COUNT=10;BYMONTHDAY=2,15".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>
            {
                startTime.Value,
                startTime.Value.AddDays(13)
            };

            /*"19971029T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19971128T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19971229T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980129T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980226T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);*/


            for (var i = 1; i < 5; i++)
            {
                var toAdd = expected.First().AddMonths(i);
                expected.Add(toAdd.AddDays(13));
                expected.Add(toAdd);
            }


            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }

        /// <summary>
        ///     Monthly on the first and last day of the month for 10 occurrences:
        ///     occurrences:
        /// </summary>
        [Fact]
        public void UnitTest17()
        {
            DateTime? startTime;
            "19970930T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=MONTHLY;COUNT=10;BYMONTHDAY=1,-1".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>
            {
                startTime.Value
            };
            DateTime? otherDT;

            "19971001T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19971031T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19971101T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19971130T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19971201T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19971231T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980101T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980131T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980201T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);

            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }

        /// <summary>
        ///     Every 18 months on the 10th thru 15th of the month for 10 occurrences:
        /// </summary>
        [Fact]
        public void UnitTest18()
        {
            DateTime? startTime;
            "19970910T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=MONTHLY;INTERVAL=18;COUNT=10;BYMONTHDAY=10,11,12,13,14,15".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>
            {
                startTime.Value
            };
            DateTime? otherDT;

            "19970911T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19970912T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19970913T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19970914T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19970915T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19990310T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19990311T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19990312T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19990313T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);


            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }

        /// <summary>
        ///     Every Tuesday, every other month:
        /// </summary>
        [Fact]
        public void UnitTest19()
        {
            DateTime? startTime;
            "19970902T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=MONTHLY;INTERVAL=2;BYDAY=TU".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>
            {
                startTime.Value
            };
            DateTime? otherDT;

            "19970909T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19970916T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19970923T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19970930T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19971104T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19971111T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19971118T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19971125T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980106T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980113T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980120T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980127T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980303T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980310T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980317T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980324T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980331T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);


            var dts = startTime.Value.ExpandTime(recurs).Take(expected.Count);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }

        /// <summary>
        ///     Yearly in June and July for 10 occurrences
        /// </summary>
        [Fact]
        public void UnitTest20()
        {
            DateTime? startTime;
            "19970610T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=YEARLY;COUNT=10;BYMONTH=6,7".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>
            {
                startTime.Value
            };
            DateTime? otherDT;


            "19970710T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980610T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980710T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19990610T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19990710T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "20000610T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "20000710T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "20010610T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "20010710T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);


            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }

        /// <summary>
        ///     Every third year on the 1st, 100th, and 200th day for 10 occurrences
        /// </summary>
        [Fact]
        public void UnitTest21()
        {
            DateTime? startTime;
            "19970101T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=YEARLY;INTERVAL=3;COUNT=10;BYYEARDAY=1,100,200".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>
            {
                startTime.Value
            };
           
            expected.Add(expected.First().AddDays(100 - 1));
            expected.Add(expected.First().AddDays(200 - 1));

            for (var i = 1; i < 4; i++)
            {
                var toAdd = expected.First().AddYears(3*i);
                var toAdd1 = toAdd.AddDays(100 - 1);
                var toAdd2 = toAdd.AddDays(200 - 1);
                expected.Add(toAdd);
                if (i == 3) break;
                expected.Add(toAdd1);
                expected.Add(toAdd2);
            }


            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }

        /// <summary>
        ///     Every other year on January, February, and March for 10 occurrences
        /// </summary>
        [Fact]
        public void UnitTest22()
        {
            DateTime? startTime;
            "19970310T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=YEARLY;INTERVAL=2;COUNT=10;BYMONTH=1,2,3".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>
            {
                startTime.Value
            };
            DateTime? otherDT;
            "19990110T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19990210T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19990310T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "20010310T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "20010110T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "20010210T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "20030210T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "20030110T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "20030310T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);

            var dts = startTime.Value.ExpandTime(recurs);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }

        /// <summary>
        ///     Every 20th Monday of the year, forever:
        /// </summary>
        [Fact]
        public void UnitTest23()
        {
            DateTime? startTime;
            "19970519T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=YEARLY;BYDAY=20MO".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>
            {
                startTime.Value
            };
            DateTime? otherDT;
            "19980518T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19990517T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);


            var dts = startTime.Value.ExpandTime(recurs).Take(3);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }

        /// <summary>
        ///     Monday of week number 20 (where the default start of the week is
        ///     Monday), forever:
        /// </summary>
        [Fact]
        public void UnitTest24()
        {
            DateTime? startTime;
            "19970512T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=YEARLY;BYWEEKNO=20;BYDAY=MO".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>
            {
                startTime.Value
            };
            DateTime? otherDT;
            "19980511T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19990517T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);


            var dts = startTime.Value.ExpandTime(recurs).Take(3);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }

        /// <summary>
        ///     Every Thursday in March, forever:
        /// </summary>
        [Fact]
        public void UnitTest25()
        {
            DateTime? startTime;
            "19970313T090000".ToDateTime(out startTime);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=YEARLY;BYMONTH=3;BYDAY=TH".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>
            {
                startTime.Value
            };
            DateTime? otherDT;
            "19970320T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19970327T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980305T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980312T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980319T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19980326T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19990311T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19990318T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19990325T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19990304T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19990318T090000".ToDateTime(out otherDT);


            var dts = startTime.Value.ExpandTime(recurs).Take(11);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }

        /// <summary>
        ///     Every Thursday, but only during June, July, and August, forever:
        /// </summary>
        [Fact]
        public void UnitTest26()
        {
            DateTime? dt;
            "19970605T090000".ToDateTime(out dt);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=YEARLY;BYDAY=TH;BYMONTH=6,7,8".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>();
            var cal = CultureInfo.InvariantCulture.Calendar;

            for (var year = 1997; year < 2000; year++)
                for (var month = 6; month < 9; month++)
                    for (var day = 1; day <= cal.GetDaysInMonth(year, month); day++)
                    {
                        var dateToAdd = new DateTime(year, month, day, dt.Value.Hour, dt.Value.Minute, dt.Value.Second);
                        if (dateToAdd.DayOfWeek == DayOfWeek.Thursday)
                            expected.Add(dateToAdd);
                    }


            var dts = dt.Value.ExpandTime(recurs).Take(expected.Count);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt1 in expected)
            {
                Assert.Contains(dt1, dts);
            }
        }

        /// <summary>
        ///     Every Friday the 13th, forever:
        /// </summary>
        [Fact]
        public void UnitTest27()
        {
            DateTime? dt;
            "19970902T090000".ToDateTime(out dt);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=MONTHLY;BYDAY=FR;BYMONTHDAY=13".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>();
            var cal = CultureInfo.InvariantCulture.Calendar;

            for (var year = 1998; year < 2001; year++)
                for (var month = 1; month < 13; month++)
                    for (var day = 1; day <= cal.GetDaysInMonth(year, month); day++)
                    {
                        if (day != 13)
                            continue;
                        var dateToAdd = new DateTime(year, month, day, dt.Value.Hour, dt.Value.Minute, dt.Value.Second);
                        if (dateToAdd.DayOfWeek == DayOfWeek.Friday)
                            expected.Add(dateToAdd);
                    }


            var dts = dt.Value.ExpandTime(recurs).Take(expected.Count);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt1 in expected)
            {
                Assert.Contains(dt1, dts);
            }
        }

        /// <summary>
        ///     The first Saturday that follows the first Sunday of the month,
        /// </summary>
        [Fact]
        public void UnitTest28()
        {
            DateTime? dt;
            "19970913T090000".ToDateTime(out dt);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=MONTHLY;BYDAY=SA;BYMONTHDAY=7,8,9,10,11,12,13".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>();
            var cal = CultureInfo.InvariantCulture.Calendar;
            var daysOfWeek = recur.ByDays.Select(x => x.DayOfWeek);
            for (var year = dt.Value.Year; year < dt.Value.Year + 10; year++)
                for (var month = 1; month < 13; month++)
                    for (var day = 1; day <= cal.GetDaysInMonth(year, month); day++)
                    {
                        if (!recur.ByMonthDay.Contains(day))
                            continue;
                        var dateToAdd = new DateTime(year, month, day, dt.Value.Hour, dt.Value.Minute, dt.Value.Second);
                        if (daysOfWeek.Contains(dateToAdd.DayOfWeek))
                            expected.Add(dateToAdd);
                    }
            expected = expected.Where(x => x >= dt).Take(10).ToList();


            var dts = dt.Value.ExpandTime(recurs).Take(expected.Count);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt1 in expected)
            {
                Assert.Contains(dt1, dts);
            }
        }

        /// <summary>
        ///     Every 4 years, the first Tuesday after a Monday in November,
        /// </summary>
        [Fact]
        public void UnitTest29()
        {
            DateTime? dt;
            "19961105T090000".ToDateTime(out dt);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=YEARLY;INTERVAL=4;BYMONTH=11;BYDAY=TU;BYMONTHDAY=2,3,4,5,6,7,8".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>();
            var cal = CultureInfo.InvariantCulture.Calendar;
            IEnumerable<DayOfWeek> daysOfWeek = null;
            if (recur.ByDays != null)
                daysOfWeek = recur.ByDays.Select(x => x.DayOfWeek);
            var interval = recur.Interval ?? 1;
            for (var year = dt.Value.Year; year < dt.Value.Year + 10; year += interval)
                for (var month = 1; month < 13; month++)
                    for (var day = 1; day <= cal.GetDaysInMonth(year, month); day++)
                    {
                        if (recur.ByMonthDay != null && !recur.ByMonthDay.Contains(day))
                            continue;
                        if (recur.ByMonth != null && !recur.ByMonth.Contains(month))
                            continue;
                        var dateToAdd = new DateTime(year, month, day, dt.Value.Hour, dt.Value.Minute, dt.Value.Second);
                        if (daysOfWeek != null && daysOfWeek.Contains(dateToAdd.DayOfWeek))
                            expected.Add(dateToAdd);
                    }
            expected = expected.Where(x => x >= dt).Take(10).ToList();


            var dts = dt.Value.ExpandTime(recurs).Take(expected.Count);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt1 in expected)
            {
                Assert.Contains(dt1, dts);
            }
        }


        /// <summary>
        ///     The third instance into the month of one of Tuesday, Wednesday, or
        ///     Thursday, for the next 3 months:
        ///     Gonna fails till is implemented ApplyBySetPos
        /// </summary>
        [Fact]
        public void UnitTest30()
        {
            DateTime? dt;
            "19970904T090000".ToDateTime(out dt);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=MONTHLY;COUNT=3;BYDAY=TU,WE,TH;BYSETPOS=3".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>();
            var cal = CultureInfo.InvariantCulture.Calendar;
            IEnumerable<DayOfWeek> daysOfWeek = null;
            if (recur.ByDays != null)
                daysOfWeek = recur.ByDays.Select(x => x.DayOfWeek);

            "19971007T090000".ToDateTime(out dt);
            expected.Add(dt.Value);
            "19971106T090000".ToDateTime(out dt);
            expected.Add(dt.Value);


            var dts = dt.Value.ExpandTime(recurs).Take(expected.Count);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt1 in expected)
            {
                Assert.Contains(dt1, dts);
            }
        }


        /// <summary>
        ///     Every 3 hours from 9:00 AM to 5:00 PM on a specific day:
        /// </summary>
        [Fact]
        public void UnitTest31()
        {
            DateTime? dt;
            "19970902T090000".ToDateTime(out dt);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=HOURLY;INTERVAL=3;UNTIL=19970902T170000Z".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>();
            DateTime? otherDT;
            var cal = CultureInfo.InvariantCulture.Calendar;

            "19970902T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19970902T120000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19970902T150000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);


            var dts = dt.Value.ExpandTime(recurs).Take(expected.Count);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt1 in expected)
            {
                Assert.Contains(dt1, dts);
            }
        }

        /// <summary>
        ///     Every 15 minutes for 6 occurrences:
        /// </summary>
        [Fact]
        public void UnitTest32()
        {
            DateTime? dt;
            "19970902T090000".ToDateTime(out dt);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=MINUTELY;INTERVAL=15;COUNT=6".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>();
            var cal = CultureInfo.InvariantCulture.Calendar;
            IEnumerable<DayOfWeek> daysOfWeek = null;
            if (recur.ByDays != null)
                daysOfWeek = recur.ByDays.Select(x => x.DayOfWeek);
            var interval = recur.Interval != null ? recur.Interval.Value : 1;
            for (var i = 0; i < 6; i++)
            {
                var dateToAdd = dt.Value.AddMinutes(i*interval);
                expected.Add(dateToAdd);
            }


            var dts = dt.Value.ExpandTime(recurs);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt1 in expected)
            {
                Assert.Contains(dt1, dts);
            }
        }


        /// <summary>
        ///     Every hour and a half for 4 occurrences:
        /// </summary>
        [Fact]
        public void UnitTest33()
        {
            DateTime? dt;
            "19970902T090000".ToDateTime(out dt);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=MINUTELY;INTERVAL=90;COUNT=4".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>();
            var cal = CultureInfo.InvariantCulture.Calendar;
            IEnumerable<DayOfWeek> daysOfWeek = null;
            if (recur.ByDays != null)
                daysOfWeek = recur.ByDays.Select(x => x.DayOfWeek);
            var interval = recur.Interval != null ? recur.Interval.Value : 1;
            for (var i = 0; i < 4; i++)
            {
                var dateToAdd = dt.Value.AddMinutes(i*interval);
                expected.Add(dateToAdd);
            }


            var dts = dt.Value.ExpandTime(recurs);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt1 in expected)
            {
                Assert.Contains(dt1, dts);
            }
        }

        /// <summary>
        ///     Every 20 minutes from 9:00 AM to 4:40 PM every day:
        /// </summary>
        [Fact]
        public void UnitTest34()
        {
            DateTime? dt;
            "19970902T090000".ToDateTime(out dt);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=DAILY;BYHOUR=9,10,11,12,13,14,15,16;BYMINUTE=0,20,40".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>();
            var cal = CultureInfo.InvariantCulture.Calendar;
            IEnumerable<DayOfWeek> daysOfWeek = null;
            if (recur.ByDays != null)
                daysOfWeek = recur.ByDays.Select(x => x.DayOfWeek);
            var interval = recur.Interval != null ? recur.Interval.Value : 1;
            for (var day = 2; day < 5; day++)
            {
                for (var hour = 9; hour < 17; hour++)
                {
                    var dateToAdd = new DateTime(dt.Value.Year, dt.Value.Month, day, hour, 0, dt.Value.Second);
                    expected.Add(dateToAdd);
                    for (var i = 0; i < 2; i ++)
                    {
                        dateToAdd = dateToAdd.AddMinutes(20);
                        expected.Add(dateToAdd);
                    }
                }
            }


            var dts = dt.Value.ExpandTime(recurs).Take(expected.Count);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt1 in expected)
            {
                Assert.Contains(dt1, dts);
            }

            "RRULE:FREQ=MINUTELY;INTERVAL=20;BYHOUR=9,10,11,12,13,14,15,16".ToRecur(out recur);
            recurs.Clear();
            recurs.Add(recur);
            dts = dt.Value.ExpandTime(recurs).Take(expected.Count);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt1 in expected)
            {
                Assert.Contains(dt1, dts);
            }
        }


        /// <summary>
        ///     An example where the days generated makes a difference because of
        ///     WKST
        /// </summary>
        [Fact]
        public void UnitTest35()
        {
            DateTime? dt;
            "19970805T090000".ToDateTime(out dt);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=WEEKLY;INTERVAL=2;COUNT=4;BYDAY=TU,SU;WKST=MO".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>();
            DateTime? otherDT;
            var cal = CultureInfo.InvariantCulture.Calendar;

            "19970805T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19970810T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19970819T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19970824T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);

            var dts = dt.Value.ExpandTime(recurs);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt1 in expected)
            {
                Assert.Contains(dt1, dts);
            }

            //changing only WKST from MO to SU, yields different results...

            "RRULE:FREQ=WEEKLY;INTERVAL=2;COUNT=4;BYDAY=TU,SU;WKST=SU".ToRecur(out recur);

            recurs.Clear();
            recurs.Add(recur);
            expected.Clear();
            "19970805T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19970817T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19970819T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "19970831T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            dts = dt.Value.ExpandTime(recurs);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt1 in expected)
            {
                Assert.Contains(dt1, dts);
            }
        }


        /// <summary>
        ///     An example where an invalid date (i.e., February 30) is ignored.
        /// </summary>
        [Fact]
        public void UnitTest36()
        {
            DateTime? dt;
            "20070115T090000".ToDateTime(out dt);
            var recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=MONTHLY;BYMONTHDAY=15,30;COUNT=5".ToRecur(out recur);

            recurs.Add(recur);
            var expected = new List<DateTime>();
            DateTime? otherDT;
            var cal = CultureInfo.InvariantCulture.Calendar;

            "20070115T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "20070130T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "20070215T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "20070315T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            "20070330T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);

            var dts = dt.Value.ExpandTime(recurs);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt1 in expected)
            {
                Assert.Contains(dt1, dts);
            }
        }
    }
}