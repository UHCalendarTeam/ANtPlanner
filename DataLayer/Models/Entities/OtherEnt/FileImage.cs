using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataLayer.Models.Entities.OtherEnt.RelationsEnt;
using DataLayer.Models.Entities.OtherEnt.Resource;
using DataLayer.Models.Entities.ResourcesAndCollections;
using DataLayer.Models.Interfaces;

namespace DataLayer.Models.Entities.OtherEnt
{
    public class FileImage : Entity
    {
        public string DisplayName
        { get; set; }

        public string Caption
        { get; set; }

        public string ImageUrl
        { get; set; }

        public ICollection<RCalendarResourcesImagenFiles> RCalendarResourcesImagenFiles { get; set; }

        public ICollection<RImagenFilesResources> RImagensFilesResources { get; set; }

        public ICollection<RImagenFilesPersons> RImagensFilesPersons { get; set; }

        public ICollection<RImagenFilesLocations> RImagenFilesLocations { get; set; }

        public FileImage()
        {

        }

        public FileImage(string url)
        {
            this.ImageUrl = url;
        }
    }
}