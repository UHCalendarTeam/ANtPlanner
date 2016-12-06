using System.Linq;
using DataLayer.Contexts;
using DataLayer.Models.Entities.OtherEnt.Resource;
using DataLayer.Models.Interfaces.Repositories;

namespace DataLayer.Models.Repositories
{
    public class ResourceTypeRepository : PropertyContainerRepository<ResourceType, string>, IResourceTypeRepository
    {
        public ResourceTypeRepository(CalDavContext context) : base(context)
        {
        }

        public bool ContainsProperty(ResourceType resourceType, string propertyName)
        {
            return resourceType.Properties.Any(rt => rt.Name.Equals(propertyName));
        }

        public bool ContainsPropertyInNameSpace(ResourceType resourceType, string propertyName, string nameSpace)
        {
            return resourceType.Properties.Any(rt => rt.Name.Equals(propertyName) && rt.Namespace.Equals(nameSpace));
        }
    }
}
