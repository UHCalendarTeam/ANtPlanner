using DataLayer.Models.Entities.OtherEnt.Resource;

namespace DataLayer.Models.Interfaces.Repositories
{
    public interface IResourceTypeRepository : IPropertyContainerRepository<ResourceType, string>
    {
        bool ContainsProperty(ResourceType resourceType,string propertyName);

        bool ContainsPropertyInNameSpace(ResourceType resourceType, string propertyName, string nameSpace);
    }
}
