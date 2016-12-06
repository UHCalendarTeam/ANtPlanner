using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.Entities.ResourcesAndCollections;

namespace DataLayer.Models.Entities.OtherEnt.RelationsEnt
{
    public class RCalendarResourcePerson
    {
        public string CalendarResourceId { get; set; }

        public string PersonId { get; set; }

        public CalendarResource CalendarResource { get; set; }

        public Person Person { get; set; }
    }
}
