using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer.Models.Entities.OtherEnt.Resource;

namespace DataLayer.Models.Interfaces.Repositories
{
    public interface IResourceRepository : IRepository<Resource, string>
    {
        Resource FindWhithResourceType(string id);

        Task<ICollection<Resource>> FindByProperty(string property);

        Task<ICollection<Resource>> FindByTYpe(string type);

        Task<ICollection<Resource>> GetByPerson(string personId);

        Task<ICollection<Resource>> GetByLocation(string locationId);

        Task<ICollection<Resource>> GetByCalendarResource(string calendarResourceId);

    }
}
