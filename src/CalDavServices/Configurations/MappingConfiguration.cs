using AutoMapper;
using DataLayer.Models;
using ICalendar.CalendarComponents;

namespace CalDavServices.Configurations
{
    public class MappingConfiguration: Profile
    {
        public MappingConfiguration()
        {
            this.CreateMap<VEvent, EasyCalendarEvent>();
        }
    }
}
