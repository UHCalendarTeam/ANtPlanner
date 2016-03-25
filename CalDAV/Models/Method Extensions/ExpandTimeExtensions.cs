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
        //TODO: Ver Adriano
        /// <summary>
        /// Expand the given dateTIme following the given rules.
        /// </summary>
        /// <param name="dateTime">The dateTime to expand.</param>
        /// <param name="ruleProperties">The set of rules to apply</param>
        /// <returns>The expanded DateTImes.</returns>
        public static IEnumerable<DateTime> ExpandTime(this DateTime dateTime, List<Recur> ruleProperties)
        {

            List<DateTime> output = new List<DateTime>();

            return ruleProperties.SelectMany(rule => dateTime.ApplyFrequency(rule));

        }

        //TODO: Ver Adriano
        private static IEnumerable<DateTime> ApplyFrequency(this DateTime dateTime, Recur rule)
        {
            var count = rule.Count ?? 1000;
            DateTime until = rule.Until ?? DateTime.Now.AddMonths(6);

            List<DateTime> output = new List<DateTime>() { dateTime };
            List<DateTime> genDTs = new List<DateTime>();
            var freq = rule.Frequency.Value;
            var interval = rule.Interval ?? 1;
            //gonna create as dates as the count specify
            for (int i = 0; i < count - 1; i++)
            {
                //take the last added item and work with it
                var lastestDate = output.Last();
                //if reach the max date the return
                if (lastestDate >= until)
                {
                    output.RemoveAt(output.Count - 1);
                    break;
                }

                switch (freq)
                {
                    case RecurValues.Frequencies.DAILY:
                        output.Add(lastestDate.AddDays(interval));
                        break;
                    case RecurValues.Frequencies.WEEKLY:
                        output.Add(lastestDate.AddDays(7 * interval));
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
                    case RecurValues.Frequencies.SECONDLY:
                        output.Add(lastestDate.AddSeconds(interval));
                        break;

                }
            }
            var result = output.ApplyByMonth(rule);

            //remove the datetimes before the DTSTART
            result = result.Where(x => x >= dateTime);
            //apply the last evaluations
            //the number of item have to be the first COUNT item
             result = result.Take(count);

            //the dts have to be less than the UNTIL value
            return result.Where(x => x <= until);
        }

        //TODO: Ver Adriano
        private static IEnumerable<DateTime> ApplyByMonth(this IEnumerable<DateTime> dateTimes, Recur rule)
        {
            if (rule.ByMonth == null)
                return dateTimes.ApplyByWeekNo(rule);

            /* List<DateTime> daysOfMonths = new List<DateTime>();
             var cal = CultureInfo.InvariantCulture.Calendar;*/

            var output = new List<DateTime>();
            //generate all the days for the given months
            //if the FREQ rule is YEARLY
            if (rule.Frequency.Value == RecurValues.Frequencies.YEARLY)
            {
                foreach (var dt in dateTimes)
                {
                    foreach (var month in rule.ByMonth)
                    {
                        var dateToAdd = new DateTime(dt.Year, month, dt.Day, dt.Hour, dt.Minute, dt.Second);
                        if (!output.Contains(dateToAdd))
                            output.Add(dateToAdd);
                    }
                }
                return output.ApplyByWeekNo(rule);
            }
            //limit the dateTimes to those that have the month
            //equal to any of the given months if the FREQ is not YEARLY
            return dateTimes.Where(x => rule.ByMonth.Contains(x.Month)).ApplyByWeekNo(rule);

        }

        //TODO: Ver Adriano
        private static IEnumerable<DateTime> ApplyByWeekNo(this IEnumerable<DateTime> dateTimes, Recur rule)
        {
            if (rule.ByWeekNo == null)
                return dateTimes.ApplyByYearDay(rule);
            //if the FREQ is other than YEARLY then do nothing
            if (rule.Frequency != null && rule.Frequency.Value != RecurValues.Frequencies.YEARLY)
                return dateTimes.ApplyByYearDay(rule);
            var cal = CultureInfo.InvariantCulture.Calendar;

            //just return the dates whos week of the year is one
            //of the specified

            return dateTimes.Where(x => rule.ByWeekNo.
                Contains(cal.GetWeekOfYear(x, CalendarWeekRule.FirstFourDayWeek, rule.Wkst )));
        }

        //TODO: Ver Adriano
        private static IEnumerable<DateTime> ApplyByYearDay(this IEnumerable<DateTime> dateTimes, Recur rule)
        {
            if (rule.ByYearDay == null ||
                rule.Frequency.Value == RecurValues.Frequencies.DAILY ||
                rule.Frequency.Value == RecurValues.Frequencies.WEEKLY ||
                rule.Frequency.Value == RecurValues.Frequencies.MONTHLY)
                return dateTimes.ApplyByMonthDay(rule);

            //limit the dateTImes if the FREQ is set to SECONDLY, MINUTELY or HOURLY
            if (rule.Frequency.Value == RecurValues.Frequencies.MINUTELY ||
                rule.Frequency.Value == RecurValues.Frequencies.HOURLY ||
                rule.Frequency.Value == RecurValues.Frequencies.SECONDLY )//TODO:NEAGATIVE number
                return dateTimes.Where(x => rule.ByYearDay.Contains(x.DayOfYear)).ApplyByMonthDay(rule);

            //expand the datetimes if the FREQ is set to YEARLY
            List<DateTime> output = new List<DateTime>();
            foreach (var dateTime in dateTimes)
                foreach (var day in rule.ByYearDay)
                {
                    //if the day is negative then create the ref date the the last day of the year,
                    //otherwise create it to the first day
                    var refDate = day > 0
                        ? new DateTime(dateTime.Year, 1, 1, dateTime.Hour, dateTime.Minute, dateTime.Second)
                        : new DateTime(dateTime.Year, 12, 31, dateTime.Hour, dateTime.Minute, dateTime.Second);
                    output.Add(refDate.AddDays(day-1));
                }


            return output.ApplyByMonthDay(rule);
        }

        //TODO: Ver Adriano
        private static IEnumerable<DateTime> ApplyByMonthDay(this IEnumerable<DateTime> dateTimes, Recur rule)
        {
            if (rule.ByMonthDay == null || rule.Frequency.Value == RecurValues.Frequencies.WEEKLY)
                return dateTimes.ApplyByDay(rule);
            var cal = CultureInfo.InvariantCulture.Calendar;

            //limit the dateTImes if the FREQ is set to SECONDLY, MINUTELY or HOURLY or DAYLY
            if (rule.Frequency.Value != RecurValues.Frequencies.MONTHLY &&
                rule.Frequency.Value != RecurValues.Frequencies.YEARLY)
                return dateTimes.Where(dateTime =>
                {
                    //if the givens ByMonthDay are negative then
                    //subtract it to the total days of the month
                    var tempDays = rule.ByMonthDay.Select(dayOfMonth =>
                    {
                        //if the day is positive dont do nothing
                        if (dayOfMonth > 0)
                            return dayOfMonth;
                        //if the day is negative then substract it to the 
                        //count of days in the month
                        var day = cal.GetDaysInMonth(dateTime.Year, dateTime.Month) + dayOfMonth;
                        //if the result day is not in the range of the month then dismiss it
                        if (day < 0)
                            return -1;
                        return day;
                    });
                    return tempDays.Contains(dateTime.Day);
                }).ApplyByDay(rule);

            //if the FREQ is MONTHLY or YEARLY then expand 
            List<DateTime> output = new List<DateTime>();
            foreach (var dateTime in dateTimes)
            {
                //if the givens ByMonthDay are negative then
                //subtract it to the total days of the month
                var tempDays = rule.ByMonthDay.Select(dayOfMonth =>
                {
                    //if the day is positive dont do nothing
                    if (dayOfMonth > 0)
                        return dayOfMonth;
                    //if the day is negative then substract it to the 
                    //count of days in the month
                    var day = cal.GetDaysInMonth(dateTime.Year, dateTime.Month) + dayOfMonth+1;
                    //if the result day is not in the range of the month then dismiss it
                    if (day < 0)
                        return -1;
                    return day;
                }).Where(x => x >= 0);
                output.AddRange(tempDays.Select(day => new DateTime(dateTime.Year, dateTime.Month, day,
                    dateTime.Hour, dateTime.Minute, dateTime.Second)));
            }
            return output.ApplyByDay(rule);

        }


        //TODO: Ver Adriano
        private static IEnumerable<DateTime> ApplyByDay(this IEnumerable<DateTime> dateTimes, Recur rule)
        {
            if (rule.ByDays == null)
                return dateTimes.ApplyByHour(rule);


            var daysOfWeek = rule.ByDays.Select(day => day.DayOfWeek).ToArray();
            //limit the dateTImes if the FREQ is set to SECONDLY, MINUTELY or HOURLY
            if (rule.Frequency.Value == RecurValues.Frequencies.MINUTELY ||
                rule.Frequency.Value == RecurValues.Frequencies.HOURLY||
                rule.Frequency.Value == RecurValues.Frequencies.SECONDLY)
                return dateTimes.Where(dateTime =>
                    daysOfWeek.Contains(dateTime.DayOfWeek)).
                    ApplyByHour(rule);

            //expand the dts to the given days of the week
            var output = new List<DateTime>();
            var cal = CultureInfo.InvariantCulture.Calendar;

            foreach (var dt in dateTimes)
            {
                switch (rule.Frequency.Value)
                {
                    case RecurValues.Frequencies.WEEKLY:
                        int weekNum = cal.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, rule.Wkst);
                        var result = dt.GenerateDayOfWeek(weekNum, rule.Wkst);
                        output.AddRange(result.Where(r => daysOfWeek.Contains(r.DayOfWeek)));
                        break;

                    case RecurValues.Frequencies.YEARLY:
                    case RecurValues.Frequencies.MONTHLY:
                        //if the BYDAYS contains integers then
                        if(rule.ByDays.Any(x=>x.OrdDay!=null))
                            output.AddRange(dt.SpecialExpandMonthDayWithInt(rule));
                        else
                            output.AddRange(dt.SpecialExpandMonth(rule));
                        break;

                }

            }

            return output.ApplyByHour(rule);

            

        }



        private static IEnumerable<DateTime> ApplyByHour(this IEnumerable<DateTime> dateTimes, Recur rule)
        {
            if (rule.ByHours== null)
                return dateTimes.ApplyByMinute(rule);


            //limit the dateTImes if the FREQ is set to SECONDLY, MINUTELY or HOURLY
            if (rule.Frequency.Value == RecurValues.Frequencies.MINUTELY ||
                rule.Frequency.Value == RecurValues.Frequencies.HOURLY||
                rule.Frequency.Value == RecurValues.Frequencies.SECONDLY)
                return dateTimes.Where(dt=>rule.ByHours.Contains(dt.Hour)).ApplyByMinute(rule);

            //expand
            List<DateTime> output = new List<DateTime>();

            foreach (var dt in dateTimes)
            {
                //generate a dt per each dt and each given hour
                output.AddRange(rule.ByHours
                    .Select(hour=>new DateTime(dt.Year, dt.Month, dt.Day,hour, dt.Minute,dt.Second)));
            }

            return output.ApplyByMinute(rule);
        }
        private static IEnumerable<DateTime> ApplyByMinute(this IEnumerable<DateTime> dateTimes, Recur rule)
        {
            if (rule.ByMinutes== null)
                return dateTimes.ApplyBySecond(rule);

            //limit the dateTImes if the FREQ is set to SECONDLY or MINUTELY //TODO: add the SECONDLY condition
            if (rule.Frequency.Value == RecurValues.Frequencies.MINUTELY)
                return dateTimes.Where(dt => rule.ByMinutes.Contains(dt.Minute)).ApplyBySecond(rule);


            //expand
            List<DateTime> output = new List<DateTime>();

            foreach (var dt in dateTimes)
            {
                //generate a dt per each dt and each given hour
                output.AddRange(rule.ByMinutes
                    .Select(minute => new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, minute, dt.Second)));
            }

            return output.ApplyBySecond(rule);
            
        }

        private static IEnumerable<DateTime> ApplyBySecond(this IEnumerable<DateTime> dateTimes, Recur rule)
        {
            if (rule.BySeconds== null)
                return dateTimes.ApplyBySetPos(rule);

            // TODO: add the SECONDLY
            if (rule.Frequency.Value == RecurValues.Frequencies.SECONDLY)
                return dateTimes.Where(dt => rule.ByMinutes.Contains(dt.Minute)).ApplyBySecond(rule);

            List<DateTime> output = new List<DateTime>();

            foreach (var dt in dateTimes)
            {
                //generate a dt per each dt and each given hour
                output.AddRange(rule.BySeconds
                    .Select(second => new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, second)));
            }

            return output.ApplyBySetPos(rule);

            
        }

        private static IEnumerable<DateTime> ApplyBySetPos(this IEnumerable<DateTime> dateTimes, Recur rule)
        {
            if (rule.BySetPos== null)
                return dateTimes;
            throw new NotImplementedException($"THe rule isnt implemented yet :(.");
        }



        


        /// <summary>
        /// Generate all the days inside the given weekNumber
        /// that are in the same week that dt
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="weekNum"></param>
        /// <param name="dw"></param>
        /// <returns></returns>
        private static IEnumerable<DateTime> GenerateDayOfWeek(this DateTime dt, int weekNum, DayOfWeek dw)
        {
            List<DateTime> output = new List<DateTime>(7) {dt};
            var cal = CultureInfo.InvariantCulture.Calendar;

           
            DateTime genDT = dt.AddDays(1);
            int currentWeekNum = cal.GetWeekOfYear(genDT, CalendarWeekRule.FirstFourDayWeek, dw);
            //add the days til get to other week
            while (currentWeekNum==weekNum)
            {
                output.Add(genDT);
                genDT = genDT.AddDays(1);
                currentWeekNum = cal.GetWeekOfYear(genDT, CalendarWeekRule.FirstFourDayWeek, dw);
            }

            genDT = dt.AddDays(-1);
            currentWeekNum = cal.GetWeekOfYear(genDT, CalendarWeekRule.FirstFourDayWeek, dw);
            //substract the days til get to other week
            while (currentWeekNum == weekNum)
            {
                output.Add(genDT);
                genDT = genDT.AddDays(-1);
                currentWeekNum = cal.GetWeekOfYear(genDT, CalendarWeekRule.FirstFourDayWeek, dw);
            }

            return output;

        }

        /// <summary>
        /// Return all the days of the month that its 
        /// DayOfWeek is contained in the specified BYDAY.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        private static IEnumerable<DateTime> SpecialExpandMonth(this DateTime dt, Recur rule)
        {
            var cal = CultureInfo.InvariantCulture.Calendar;
            var daysOfMonth = cal.GetDaysInMonth(dt.Year, dt.Month);
            var daysOfWeek = rule.ByDays.Select(day => day.DayOfWeek);
            
            List<DateTime> output = new List<DateTime>(daysOfMonth);

            for (int day = 1; day <= daysOfMonth; day++)
            {
                //construct the datetime and check if the day of the week of one
                //of the specified in the BYDAY property
                var dateToAdd = new DateTime(dt.Year, dt.Month, day, dt.Hour, dt.Minute, dt.Second);
                if(daysOfWeek.Contains(dateToAdd.DayOfWeek))
                    output.Add(dateToAdd);
            }
            return output;
        } 

        /// <summary>
        /// Return all the days of the month that its 
        /// DayOfWeek is contained in the specified BYDAY when these contains
        /// ordinary values (i.e: BYDAY=1MO,-1MO).
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        private static IEnumerable<DateTime> SpecialExpandMonthDayWithInt(this DateTime dt, Recur rule)
        {
            var cal = CultureInfo.InvariantCulture.Calendar;
            var daysOfMonth = cal.GetDaysInMonth(dt.Year, dt.Month);
            var daysOfWeek = rule.ByDays;
            
            List<DateTime> output = new List<DateTime>(daysOfMonth);

            //Contains the ocurrence number of the DayOfWeek of the especific datetime
            Dictionary<DateTime, int> dayOfWeekOcurrence = new Dictionary<DateTime, int>();

            //Contains the count of ocurrence of the DayOfWeek
            Dictionary<DayOfWeek,int> daysOfWeekCount = new Dictionary<DayOfWeek, int>(7)
            {
                { DayOfWeek.Sunday, 0},
                {DayOfWeek.Monday, 0 },
                {DayOfWeek.Tuesday, 0 },
                {DayOfWeek.Wednesday, 0 },
                {DayOfWeek.Thursday, 0 },
                {DayOfWeek.Friday, 0 },
                {DayOfWeek.Saturday, 0 }
            };
            

            for (int day = 1; day <= daysOfMonth; day++)
            {
                //construct the datetime and check if the day of the week of one
                //of the specified in the BYDAY property
                var dtToAdd = new DateTime(dt.Year, dt.Month, day, dt.Hour, dt.Minute, dt.Second);
                dayOfWeekOcurrence.Add(dtToAdd,daysOfWeekCount[dtToAdd.DayOfWeek]+1);
                daysOfWeekCount[dtToAdd.DayOfWeek] += 1;
            }

            //iterate over the datetimes and see if its DayOfWeek
            //is one of the given in the rule.BYDAY
            foreach (var dateTime in dayOfWeekOcurrence.Keys)
                foreach (var wDay in daysOfWeek)
                {
                    //if the wDay integer is positive and is the same of the current dateTime
                    //and its DayOfWeek is the same of wDay then add it
                    if(wDay.OrdDay.Value>0)
                        if (dateTime.DayOfWeek == wDay.DayOfWeek
                            && dayOfWeekOcurrence[dateTime] == wDay.OrdDay.Value)
                            output.Add(dateTime);
                        else
                            continue; 
                        
                        //if is negative and the total count of datetime.DayOfWeek
                    else if(dateTime.DayOfWeek == wDay.DayOfWeek&&
                       daysOfWeekCount[dateTime.DayOfWeek]- dayOfWeekOcurrence[dateTime]+1 == wDay.OrdDay.Value*-1)
                        output.Add(dateTime);
                }

            return output;
        } 






        



    }
}
