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
        /// <summary>
        /// Daily for 10 occurrences:
        /// </summary>
        [Fact]
        public void UnitTest1()
        {
            DateTime? startTime;
            "19970902T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
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
            List<Recur> recurs = new List<Recur>();
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
            List<Recur> recurs = new List<Recur>();
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
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "FREQ=DAILY;INTERVAL=10;COUNT=5".ToRecur(out recur);

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
                Assert.Contains(dt, dts);
            }


        }

        /// <summary>
        /// Every day in January, for 3 years:
        /// </summary>
        [Fact]
        public void UnitTest5()
        {
            DateTime? startTime;
            "19980101T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "FREQ=YEARLY;UNTIL=20000131T140000Z;BYMONTH=1;BYDAY=SU,MO,TU,WE,TH,FR,SA".ToRecur(out recur);

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
                Assert.Contains(dt, dts);
            }


        }

        /// <summary>
        /// Every day in January, for 3 years:
        /// </summary>
        [Fact]
        public void UnitTest6()
        {
            DateTime? startTime;
            "19980101T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "FREQ=DAILY;UNTIL=20000131T140000Z;BYMONTH=1".ToRecur(out recur);

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
                Assert.Contains(dt, dts);
            }


        }

        /// <summary>
        ///Weekly for 10 occurrences
        /// </summary>
        [Fact]
        public void UnitTest7()
        {
            DateTime? startTime;
            "19970902T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "FREQ=WEEKLY;COUNT=10".ToRecur(out recur);

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
                Assert.Contains(dt, dts);
            }


        }

        /// <summary>
        ///Weekly until December 24, 1997
        /// </summary>
        [Fact]
        public void UnitTest8()
        {
            DateTime? startTime;
            "19970902T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "FREQ=WEEKLY;UNTIL=19971224T000000Z".ToRecur(out recur);

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
                Assert.Contains(dt, dts);
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
            "FREQ=WEEKLY;UNTIL=19971007T000000Z;WKST=SU;BYDAY=TU,TH".ToRecur(out recur);

            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>() {startTime.Value};

            for (int i = 1; i < 10; i++)
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

            recurs = new List<Recur>();
            recurs.Add(recur);
            dts = startTime.Value.ExpandTime(recurs).ToList();
            Assert.Equal(10, dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }
        }


        /// <summary>
        ///Every other week on Monday, Wednesday, and Friday until December
        //  24, 1997, starting on Monday, September 1, 1997:
        /// </summary>
        [Fact]
        public void UnitTest10()
        {
            DateTime? startTime;
            "19970901T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "FREQ=WEEKLY;INTERVAL=2;UNTIL=19971224T000000Z;WKST=SU;BYDAY=MO,WE,FR".ToRecur(out recur);

            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>() {startTime.Value};

            for (int i = 1; i < 25; i++)
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

        /// <summary>
        ///Every other week on Monday, Wednesday, and Friday until December
        //  Every other week on Tuesday and Thursday, for 8 occurrences:
        /// </summary>
        [Fact]
        public void UnitTest11()
        {
            DateTime? startTime;
            "19970902T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "FREQ=WEEKLY;INTERVAL=2;COUNT=8;WKST=SU;BYDAY=TU,TH".ToRecur(out recur);

            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>() {startTime.Value};

            for (int i = 1; i < 8; i++)
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
        ///Monthly on the first Friday until December 24, 1997:
        /// </summary>
        [Fact]
        public void UnitTest12()
        {
            DateTime? startTime;
            "19970905T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "FREQ=MONTHLY;UNTIL=19971224T000000Z;BYDAY=1FR".ToRecur(out recur);

            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>()
            {
                startTime.Value,


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
        ///Every other month on the first and last Sunday of the month for 10
        ///occurrences:
        /// </summary>
        [Fact]
        public void UnitTest13()
        {
            DateTime? startTime;
            "19970907T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "FREQ=MONTHLY;INTERVAL=2;COUNT=10;BYDAY=1SU,-1SU".ToRecur(out recur);

            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>()
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
        ///Monthly on the second-to-last Monday of the month for 6 months:
        ///occurrences:
        /// </summary>
        [Fact]
        public void UnitTest14()
        {
            DateTime? startTime;
            "19970922T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=MONTHLY;COUNT=6;BYDAY=-2MO".ToRecur(out recur);

            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>()
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
        ///Monthly on the third-to-the-last day of the month, forever:
        ///occurrences:
        /// </summary>
        [Fact]
        public void UnitTest15()
        {
            DateTime? startTime;
            "19970928T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=MONTHLY;BYMONTHDAY=-3".ToRecur(out recur);

            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>()
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
        ///Monthly on the 2nd and 15th of the month for 10 occurrences:
        ///occurrences:
        /// </summary>
        [Fact]
        public void UnitTest16()
        {
            DateTime? startTime;
            "19970902T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=MONTHLY;COUNT=10;BYMONTHDAY=2,15".ToRecur(out recur);

            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>()
            {
                startTime.Value,
                startTime.Value.AddDays(13)
            };
            DateTime? otherDT;
            
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



              for (int i = 1; i < 5; i++)
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
        ///Monthly on the first and last day of the month for 10 occurrences:
        ///occurrences:
        /// </summary>
        [Fact]
        public void UnitTest17()
        {
            DateTime? startTime;
            "19970930T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=MONTHLY;COUNT=10;BYMONTHDAY=1,-1".ToRecur(out recur);

            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>()
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
        ///Every 18 months on the 10th thru 15th of the month for 10 occurrences:
        /// </summary>
        [Fact]
        public void UnitTest18()
        {
            DateTime? startTime;
            "19970910T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=MONTHLY;INTERVAL=18;COUNT=10;BYMONTHDAY=10,11,12,13,14,15".ToRecur(out recur);

            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>()
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
        ///Every Tuesday, every other month:
        /// </summary>
        [Fact]
        public void UnitTest19()
        {
            DateTime? startTime;
            "19970902T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=MONTHLY;INTERVAL=2;BYDAY=TU".ToRecur(out recur);

            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>()
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
        ///Yearly in June and July for 10 occurrences
        /// </summary>
        [Fact]
        public void UnitTest20()
        {
            DateTime? startTime;
            "19970610T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=YEARLY;COUNT=10;BYMONTH=6,7".ToRecur(out recur);

            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>()
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
        ///Every third year on the 1st, 100th, and 200th day for 10 occurrences
        /// </summary>
        [Fact]
        public void UnitTest21()
        {
            DateTime? startTime;
            "19970101T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=YEARLY;INTERVAL=3;COUNT=10;BYYEARDAY=1,100,200".ToRecur(out recur);

            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>()
            {
                startTime.Value
            };
            DateTime? otherDT;

            expected.Add(expected.First().AddDays(100-1));
            expected.Add(expected.First().AddDays(200-1));

            for (int i = 1; i < 4; i++)
            {
                var toAdd = expected.First().AddYears(3*i);
                var toAdd1 = toAdd.AddDays(100-1);
                var toAdd2 = toAdd.AddDays(200-1);
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
        ///Every other year on January, February, and March for 10 occurrences
        /// </summary>
        [Fact]
        public void UnitTest22()
        {
            DateTime? startTime;
            "19970310T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=YEARLY;INTERVAL=2;COUNT=10;BYMONTH=1,2,3".ToRecur(out recur);

            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>()
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
        ///Every 20th Monday of the year, forever:
        /// </summary>
        [Fact]
        public void UnitTest23()
        {
            DateTime? startTime;
            "19970519T090000".ToDateTime(out startTime);
            List<Recur> recurs = new List<Recur>();
            Recur recur;
            "RRULE:FREQ=YEARLY;BYDAY=20MO".ToRecur(out recur);

            recurs.Add(recur);
            List<DateTime> expected = new List<DateTime>()
            {
                startTime.Value
            };
            DateTime? otherDT;
            "19980518T090000".ToDateTime(out otherDT);
            "19990517T090000".ToDateTime(out otherDT);
            expected.Add(otherDT.Value);
            

            var dts = startTime.Value.ExpandTime(recurs).Take(3);
            Assert.Equal(expected.Count(), dts.Count());
            foreach (var dt in expected)
            {
                Assert.Contains(dt, dts);
            }

        }





    }
}
