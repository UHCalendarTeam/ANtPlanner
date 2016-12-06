using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Models.Entities.OtherEnt.RelationsEnt
{
    public class RImagenFilesPersons
    {
        public string ImageFileId { get; set; }

        public string PersonId { get; set; }

        public FileImage FileImage { get; set; }

        public Person Person { get; set; }
    }
}
