using System.Collections.Generic;
using DataLayer.Models.Entities.OtherEnt.RelationsEnt;
using DataLayer.Models.Entities.ResourcesAndCollections;
using DataLayer.Models.Entities.OtherEnt.Resource;
using DataLayer.Models.Interfaces;

namespace DataLayer.Models.Entities.OtherEnt
{
    public class Location:Entity, IPropertyContainer
    {
        public string DisplayName
        { get; set; }

        public string Description
        { get; set; }

        public string Phone
        { get; set; }

        public string Email
        { get; set; }

        public int Classification
        { get; set; }

        public float Price
        { get; set; }

        public ICollection<RCalendarResourceLocation> RCalendarResourceLocations
        { get; set; }

        public ICollection<RPersonLocation> RPersonLocation
        { get; set; }

        public ICollection<RLocationResource> RLocationResources
        { get; set; }

        public ICollection<RImagenFilesLocations> RImagenFilesLocations
        { get; set; }

        public ICollection<Property> Properties
        { get; set; }

        public Location()
        {

        }

        public Location(string name, string description, string phone, string email, int classification, int price)
        {
            this.DisplayName = name;
            this.Description = description;
            this.Phone = phone;
            this.Email = email;
            this.Classification = classification;
            this.Price = price;
        }

    }
}
