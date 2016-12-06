using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DataLayer.Models.Entities.ResourcesAndCollections;

namespace DataLayer.Models.Entities.OtherEnt.RelationsEnt
{
    public class RCalendarResourceLocation
    {
        public string CalendarResoureId { get; set; }

        public string LocationId { get; set; }

        public CalendarResource CalendarResource { get; set; }

        public Location Location { get; set; }
    }
}
