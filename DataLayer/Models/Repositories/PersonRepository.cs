using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Contexts;
using DataLayer.Models.Entities.OtherEnt;
using DataLayer.Models.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models.Repositories
{
    public class PersonRepository : PropertyContainerRepository<Person, string>, IPersonRepository
    {
        public PersonRepository(CalDavContext context) : base(context)
        {
        }

        public async Task<Person> FindByDisplayName(string name)
        {
            return await DbSet.FirstOrDefaultAsync(p => p.DisplayName.Equals(name));
        }

        public async Task<ICollection<Person>> FindByAge(int age)
        {
            return await DbSet.Where(p => p.Age == age).ToListAsync();
        }

        public async Task<Person> FindByPhone(string phone)
        {
            return await DbSet.FirstOrDefaultAsync(p => p.Phone.Equals(phone));
        }

        public async Task<Person> FindByEmail(string email)
        {
            return await DbSet.FirstOrDefaultAsync(p => p.Email.Equals(email));
        }

        public async Task<ICollection<Person>> FindByHigherAge(int age)
        {
            return await DbSet.Where(p => p.Age >= age).OrderBy(x => x.DisplayName).ToListAsync();
        }

        public async Task<ICollection<Person>> FindByShortAge(int age)
        {
            return await DbSet.Where(p => p.Age < age).OrderBy(x => x.DisplayName).ToListAsync();
        }

        public async Task<ICollection<Person>> FindByRole(PersonRoles role)
        {
            return await DbSet.Where(p => p.Role == role).OrderBy(x => x.DisplayName).ToListAsync();
        }

        public async Task<ICollection<Person>> FindByLanguages(Languages language)
        {
            return await DbSet.Where(p => p.Languageses.Contains(language)).OrderBy(x => x.DisplayName).ToListAsync();
        }

        public async Task<ICollection<Person>> GetByCalendarResource(string calendarResourceId)
        {
            return await Task.Factory.StartNew(() =>
         DbSet.Where(fi =>
        fi.RCalendarResourcePerson.Any(ip =>
        ip.CalendarResourceId.Equals(calendarResourceId))).OrderBy(x => x.DisplayName).ToList());
        }

        public async Task<ICollection<Person>> GetByResource(string resourceId)
        {
            return await Task.Factory.StartNew(() =>
          DbSet.Where(fi =>
         fi.RPersonResource.Any(ip =>
         ip.ResourceId.Equals(resourceId))).OrderBy(x => x.DisplayName).ToList());
        }

        public async Task<ICollection<Person>> GetByFileImage(string fileImageId)
        {
            return await Task.Factory.StartNew(() =>
         DbSet.Where(fi =>
        fi.RImagenFilesPersons.Any(ip =>
        ip.ImageFileId.Equals(fileImageId))).OrderBy(x => x.DisplayName).ToList());
        }
    }
}
