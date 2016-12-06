using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Models.Entities.OtherEnt.RelationsEnt
{
    public class RPersonLocation
    {
        public string PersonId { get; set; }

        public string LocationId { get; set; }

        public Person Person { get; set; }

        public Location Location { get; set; }
    }
}
