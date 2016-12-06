using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataLayer.Models.Interfaces;

namespace DataLayer.Models.Entities.OtherEnt.Resource
{
    public class ResourceType : Entity, IPropertyContainer
    {
        public string Type { get; set; }


        public ICollection<Resource> Resources { get; set; }

        public ICollection<Property> Properties { get; set; }

        public ResourceType()
        {

        }

        public ResourceType(string type, params Property[] properties)
        {
            this.Properties = new List<Property>(properties);
            this.Type = type;
        }


        public override bool Equals(object obj)
        {
            ResourceType other = obj as ResourceType;

            if (ReferenceEquals(other, null))
                return false;

            if (other.Properties.Count != Properties.Count)
                return false;

            foreach (Property property in Properties)
            {
                if (!other.Properties.Contains(property))
                    return false;
            }
            return true;
        }
    }
}
