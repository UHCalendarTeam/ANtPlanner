using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ICalendar.ComponentProperties;
using ICalendar.ValueTypes;
using Remotion.Linq.Utilities;

namespace CalDAV.Models.Method_Extensions
{
    /// <summary>
    /// This class contains the necessary methods to expand t
    /// a dateTime by a RRULE
    /// </summary>
    public static class ExpandTimeExtensions
    {
        public static List<DateTime> Expand(this DateTime dateTime, List<Rrule> ruleProperties)
        {
            foreach (var ruleProperty in ruleProperties)
            {
                var rule = ruleProperty.Value;
               
            }
            return null;
        }


        private static List<DateTime> ApplyFrequency(this DateTime dateTime, RecurValues.Frequencies freq, int count,
            DateTime until, int interval)
        {

            List<DateTime> output = new List<DateTime>() {dateTime};
            //gonna create as dates as the count specify
            for (int i = 0; i < count; i++)
            {
                //take the last added item and work with it
                var lastestDate = output.Last();
                //if reach the max date the return
                if (lastestDate >= until)
                    return output;

                switch (freq)
                {
                    case RecurValues.Frequencies.DAILY:
                        output.Add(lastestDate.AddDays(interval));
                        break;
                    case RecurValues.Frequencies.WEEKLY:
                        output.Add(lastestDate.AddDays(7*interval));
                        break;
                    case RecurValues.Frequencies.MONTHLY:
                        output.Add(lastestDate.AddMonths(interval));
                        break;
                    case RecurValues.Frequencies.YEARLY:
                        output.Add(lastestDate.AddYears(interval));
                        break;
                    case RecurValues.Frequencies.HOURLY:
                        output.Add(lastestDate.AddHours(interval));
                        break;
                    case RecurValues.Frequencies.MINUTELY:
                        output.Add(lastestDate.AddMinutes(interval));
                        break;

                }
            }
            return output;
        }


        private static IEnumerable<DateTime> AppplyByMonth(this IEnumerable<DateTime> dateTimes, Recur rule)
        {
            if (rule.ByMonth.Length == 0)
                return dateTimes.ApplyByWeekNo(rule);
            
            List<DateTime> daysOfMonths = new List<DateTime>();
            var cal = CultureInfo.InvariantCulture.Calendar;


            //generate all the days for the given months
            //if the FREQ rule is YEARLY
            if(rule.Frequency.Value==RecurValues.Frequencies.YEARLY)
            {
                foreach (var dateTime in dateTimes)
                {
                    foreach (var month in rule.ByMonth)
                    {
                        var daysInMonth = cal.GetDaysInMonth(dateTime.Year, month);
                        for (int day = 0; day < daysInMonth; day++)
                        {
                            daysOfMonths.Add(new DateTime(dateTime.Year, month, day));
                        }
                    }
                }
                return daysOfMonths.ApplyByWeekNo(rule);
            }
            //limit the dateTimes to those that have the month
            //equal to any of the given months if the FREQ is not YEARLY
            return dateTimes.Where(x => rule.ByMonth.Contains(x.Month)).ApplyByWeekNo(rule);

        }


        private static IEnumerable<DateTime> ApplyByWeekNo(this IEnumerable<DateTime> dateTimes, Recur rule)
        {
            if (rule.ByWeekNo.Length == 0)
                return dateTimes.ApplyByYearDay(rule);
            //if the FREQ is other than YEARLY then do nothing
            if(rule.Frequency.Value!=null &&rule.Frequency.Value!=RecurValues.Frequencies.YEARLY)
                return dateTimes.ApplyByYearDay(rule);
            var cal = CultureInfo.InvariantCulture.Calendar;
            
            //just return the dates whos week of the year is one
            //of the specified
            
            return dateTimes.Where(x => rule.ByWeekNo.
                Contains(cal.GetWeekOfYear(x, CalendarWeekRule.FirstFourDayWeek, ToDayOfWeek(rule.Wkst.Value))));
        }


        private static IEnumerable<DateTime> ApplyByYearDay(this IEnumerable<DateTime> dateTimes, Recur rule)
        {
            if (rule.ByYearDay.Length == 0||
                rule.Frequency.Value==RecurValues.Frequencies.DAILY||
                rule.Frequency.Value == RecurValues.Frequencies.WEEKLY||
                rule.Frequency.Value == RecurValues.Frequencies.MONTHLY)
                return dateTimes.ApplyByMonthDay(rule);

            //limit the dateTImes if the FREQ is set to SECONDLY, MINUTELY or HOURLY
            if (rule.Frequency.Value == RecurValues.Frequencies.MINUTELY ||
                rule.Frequency.Value == RecurValues.Frequencies.HOURLY )//TODO: add for SECONDLY when is implemented
                return dateTimes.Where(x => rule.ByYearDay.Contains(x.DayOfYear)).ApplyByMonthDay(rule);

            //expand the datetimes if the FREQ is set to YEARLY
            List<DateTime> output = new List<DateTime>();
            foreach (var dateTime in dateTimes)
            {
                output.AddRange(rule.ByYearDay.Select(day=>new DateTime(dateTime.Year)));
            }
            return output;
        }


        private static IEnumerable<DateTime> ApplyByMonthDay(this IEnumerable<DateTime> dateTimes, Recur rule)
        {
            if (rule.ByMonthDay.Length == 0)
                return dateTimes.ApplyByDay(rule);
            throw  new NotImplementedException();
        }



        private static IEnumerable<DateTime> ApplyByDay(this IEnumerable<DateTime> dateTimes, Recur rule)
        {
            if (rule.ByDays.Length == 0)
                return dateTimes.ApplyByHour(rule);
            throw new NotImplementedException();
        }



        private static IEnumerable<DateTime> ApplyByHour(this IEnumerable<DateTime> dateTimes, Recur rule)
        {
            if (rule.ByHours.Length == 0)
                return dateTimes.ApplyByMinute(rule);
            throw new NotImplementedException();
        }
        private static IEnumerable<DateTime> ApplyByMinute(this IEnumerable<DateTime> dateTimes, Recur rule)
        {
            if (rule.ByMinutes.Length == 0)
                return dateTimes.ApplyBySecond(rule);
            throw new NotImplementedException();
        }

        private static IEnumerable<DateTime> ApplyBySecond(this IEnumerable<DateTime> dateTimes, Recur rule)
        {
            if (rule.BySeconds.Length == 0)
                return dateTimes.ApplyBySetPos(rule);
            throw new NotImplementedException();
        }

        private static IEnumerable<DateTime> ApplyBySetPos(this IEnumerable<DateTime> dateTimes, Recur rule)
        {
            if (rule.ByYearDay.Length == 0)
                return dateTimes.ApplyBySetPos(rule);
            throw new NotImplementedException();
        }



        private static DayOfWeek ToDayOfWeek(RecurValues.Weekday day)
        {
            switch (day)
            {
                case RecurValues.Weekday.SU:
                    return DayOfWeek.Sunday;
                case RecurValues.Weekday.MO:
                    return DayOfWeek.Monday;
                case RecurValues.Weekday.TU:
                    return DayOfWeek.Tuesday;
                case RecurValues.Weekday.WE:
                    return DayOfWeek.Wednesday;
                case RecurValues.Weekday.TH:
                    return DayOfWeek.Thursday;
                case RecurValues.Weekday.FR:
                    return DayOfWeek.Friday;
                case RecurValues.Weekday.SA:
                    return DayOfWeek.Saturday;
                default:
                    return DayOfWeek.Sunday;
            }
        }



        



    }
}
