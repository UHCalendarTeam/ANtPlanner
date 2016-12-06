using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Models.Entities.OtherEnt.RelationsEnt
{
    public class RImagenFilesResources
    {
        public string ImageFilesId { get; set; }

        public string ResourceId { get; set; }

        public FileImage FileImage { get; set; }

        public Resource.Resource Resource { get; set; }
    }
}
