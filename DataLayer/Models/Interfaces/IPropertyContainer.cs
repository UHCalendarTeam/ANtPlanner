using System.Collections.Generic;
using DataLayer.Models.Entities;

namespace DataLayer.Models.Interfaces
{
    /// objetc whith collection the Property.
    public interface IPropertyContainer
    {
        ICollection<Property> Properties { get; set; }
    }
}
