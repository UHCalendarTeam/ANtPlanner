using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.Entities.ResourcesAndCollections;

namespace DataLayer.Models.Entities.OtherEnt.RelationsEnt
{
    public class RCalendarResourcesImagenFiles
    {
        public RCalendarResourcesImagenFiles()
        {
            
        }

        public string ImageFilesId { get; set; }

        public string CalendarResourceId { get; set; }

        public FileImage FileImage { get; set; }

        public CalendarResource CalendarResource { get; set; }
    }
}
