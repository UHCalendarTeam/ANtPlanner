using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Models.Entities.OtherEnt.RelationsEnt
{
    public class RImagenFilesLocations
    {
        public string ImageFilesId { get; set; }

        public string LocationId { get; set; }

        public FileImage FileImage { get; set; }

        public Location Location { get; set; }
    }
}
