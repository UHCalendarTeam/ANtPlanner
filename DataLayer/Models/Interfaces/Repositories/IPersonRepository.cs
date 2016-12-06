using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer.Models.Entities.OtherEnt;
using DataLayer.Models.Entities.ResourcesAndCollections;

namespace DataLayer.Models.Interfaces.Repositories
{
    public interface IPersonRepository : IPropertyContainerRepository<Person, string>
    {
        Task<Person> FindByDisplayName(string name);

        Task<ICollection<Person>> FindByAge(int age);

        Task<Person> FindByPhone(string phone);

        Task<Person> FindByEmail(string email);

        Task<ICollection<Person>> FindByHigherAge(int age);

        Task<ICollection<Person>> FindByShortAge(int age);

        Task<ICollection<Person>> FindByRole(PersonRoles role);

        Task<ICollection<Person>> FindByLanguages(Languages language);

        Task<ICollection<Person>> GetByCalendarResource(string calendarResourceId);

        Task<ICollection<Person>> GetByResource(string resourceId);

        Task<ICollection<Person>> GetByFileImage(string fileImageId);
    }
}
