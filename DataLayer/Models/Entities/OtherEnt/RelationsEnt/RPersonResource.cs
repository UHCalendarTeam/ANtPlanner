using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Models.Entities.OtherEnt.RelationsEnt
{
    public class RPersonResource
    {
        public string PersonId { get; set; }

        public string ResourceId { get; set; }

        public Person Person { get; set; }

        public Resource.Resource Resource { get; set; }
    }
}
