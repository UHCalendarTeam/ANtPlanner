using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer.Models.Entities.OtherEnt;
using DataLayer.Models.Entities.ResourcesAndCollections;

namespace DataLayer.Models.Interfaces.Repositories
{
    public interface ILocationRepository:IPropertyContainerRepository<Location,string>
    {
        Task<Location> FindByDisplayName(string name);

        Task<Location> FindByPhone(string phone);

        Task<Location> FindByEmail(string email);

        Task<ICollection<Location>> FindByClassification(int classification);

        Task<ICollection<Location>> FindByPrice(float price);

        Task<ICollection<Location>> FindByHigherPrice(float price);

        Task<ICollection<Location>> FindByShortClassification(int classification);

        Task<ICollection<Location>> FindByHigherClassification(float classification);

        Task<ICollection<Location>> FindByShortPrice(float price);

        Task<ICollection<Location>> GetByPerson(string personId);

        Task<ICollection<Location>> GetByResource(string resourceId);

        Task<ICollection<Location>> GetByFileImage(string fileImageId);

        Task<ICollection<Location>> GetByCalendarResource(string calendarResourceId);



    }
}
