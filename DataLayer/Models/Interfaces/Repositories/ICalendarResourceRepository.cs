using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer.Models.Entities.ResourcesAndCollections;

namespace DataLayer.Models.Interfaces.Repositories
{
    public interface ICalendarResourceRepository :IUrlContainerRepository<CalendarResource>, IPropertyContainerRepository<CalendarResource, string>
    {
        Task<bool> CreatePropertyForResource(CalendarResource resource, string propName, string propNs,
          string propValue, Stack<string> errorStack,
          bool adminPrivilege);

        Task<bool> ExistByStringIs(string identifier);

        Task<ICollection<CalendarResource>> GetByPerson(string personId);

        Task<ICollection<CalendarResource>> GetByResource(string resourceId);

        Task<ICollection<CalendarResource>> GetByFileImage(string fileImageId);

        Task<ICollection<CalendarResource>> GetByLocation(string locationId);

        Task<ICollection<CalendarResource>> GetByCalendarResource(string calendarResourceId);
    }
}
