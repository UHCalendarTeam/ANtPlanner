using System;
using System.Collections.Generic;
using DataLayer.Models.Entities.OtherEnt.RelationsEnt;
using DataLayer.Models.Entities.ResourcesAndCollections;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Models.Interfaces;

namespace DataLayer.Models.Entities.OtherEnt.Resource
{
    public class Resource : Entity
    {
        public string ResourceId { get; set; }

        public string ResourceTypeId { get; set; }

        public ResourceType ResourceType { get; set; }

        public ICollection<RResourceCalendarResource> ResourceCalendarResources
        { get; set; }

        public ICollection<RLocationResource> RLocationResources { get; set; }

        public ICollection<RPersonResource> RPersonResource { get; set; }

        public ICollection<RImagenFilesResources> RImagenFilesResource { get; set; }

        public Resource()
        {

        }

        public Resource(ResourceType type)
        {
            ResourceType = type;
        }
    }
}
