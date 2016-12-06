using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Models.Entities.OtherEnt.RelationsEnt
{
    public class RLocationResource
    {
        public string LocationId { get; set; }

        public string ResourceId { get; set; }

        public Location Location { get; set; }

        public Resource.Resource Resource { get; set; }
    }
}
