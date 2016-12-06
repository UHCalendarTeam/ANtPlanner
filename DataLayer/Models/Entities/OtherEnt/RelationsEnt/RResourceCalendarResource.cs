using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.Entities.ResourcesAndCollections;

namespace DataLayer.Models.Entities.OtherEnt.RelationsEnt
{
    public class RResourceCalendarResource
    {
        public string CalendarResourceId { get; set; }

        public string ResourceId { get; set; }

        public CalendarResource CalendarResource { get; set; }

        public Resource.Resource Resource { get; set; }
    }
}
