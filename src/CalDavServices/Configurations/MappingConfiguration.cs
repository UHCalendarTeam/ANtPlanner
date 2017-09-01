using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataLayer.Models.Entities.ResourcesAndCollections;
using DataLayer.Models.NonMappedEntities;
using ICalendar.Calendar;
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
