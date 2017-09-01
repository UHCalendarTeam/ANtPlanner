using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalDAV.Core.Method_Extensions;
using DataLayer.Models.NonMappedEntities;
using ICalendar.Calendar;
using ICalendar.CalendarComponents;
using ICalendar.GeneralInterfaces;
using ICalendar.Utils;

namespace CalDAV.Utils
{
    public static class ManualMapperExtentions
    {
        public static EasyCalendarEvent ToEasyCalendarEvent(this VCalendar vCalendar)
        {
            IList<ICalendarComponent> vEvents = vCalendar.GetCalendarComponents("VEVENT");
            var vEvent = vEvents.FirstOrDefault();
            var easyEvent = new EasyCalendarEvent();
            easyEvent.title = vEvent.GetComponentProperty("SUMMARY")?.StringValue;
            easyEvent.description = vEvent.GetComponentProperty("DESCRIPTION")?.StringValue;
            DateTime? start = null;
            DateTime? end = null;
            vEvent.GetComponentProperty("DTSTART")?.StringValue?.ToDateTime(out start);
            vEvent.GetComponentProperty("DTEND")?.StringValue?.ToDateTime(out end);
            easyEvent.start = start?.ToString("yyyy-MM-dd HH:mm");
            easyEvent.end = end?.ToString("yyyy-MM-dd HH:mm");
            
            return easyEvent;

        }
    }
}
