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
    public class LocationRepository : PropertyContainerRepository<Location, string>, ILocationRepository
    {
        public LocationRepository(CalDavContext context) : base(context)
        {
        }

        public async Task<Location> FindByDisplayName(string name)
        {
            return await DbSet.FirstOrDefaultAsync(l => l.DisplayName.Equals(name));
        }

        public async Task<Location> FindByPhone(string phone)
        {
            return await DbSet.FirstOrDefaultAsync(l => l.Phone.Equals(phone));
        }

        public async Task<Location> FindByEmail(string email)
        {
            return await DbSet.FirstOrDefaultAsync(l => l.Email.Equals(email));
        }

        public async Task<ICollection<Location>> FindByClassification(int classification)
        {
            return await DbSet.Where(p => p.Classification == classification)
                .OrderBy(x=>x.DisplayName).ToListAsync();
        }

        public async Task<ICollection<Location>> FindByPrice(float price)
        {
            return await DbSet.Where(p => Math.Abs(p.Price - price) < 0.5)
                .OrderBy(x => x.DisplayName).ToListAsync();
        }

        public async Task<ICollection<Location>> FindByHigherPrice(float price)
        {
            return await DbSet.Where(p => p.Price >= price)
                .OrderBy(x => x.DisplayName).ToListAsync();
        }

        public async Task<ICollection<Location>> FindByShortClassification(int classification)
        {
            return await DbSet.Where(p => p.Classification <= classification)
                .OrderBy(x => x.DisplayName).ToListAsync();
        }

        public async Task<ICollection<Location>> FindByHigherClassification(float classification)
        {
            return await DbSet.Where(p => p.Classification >= classification)
                .OrderBy(x => x.DisplayName).ToListAsync();
        }

        public async Task<ICollection<Location>> FindByShortPrice(float price)
        {
            return await DbSet.Where(p => p.Price <= price)
                .OrderBy(x => x.DisplayName).ToListAsync();
        }

        public async Task<ICollection<Location>> GetByPerson(string personId)
        {
            return await Task.Factory.StartNew(() =>
          DbSet.Where(fi =>
          fi.RPersonLocation.Any(ip =>
          ip.PersonId.Equals(personId))).OrderBy(x => x.DisplayName).ToList());
        }

        public async Task<ICollection<Location>> GetByResource(string resourceId)
        {
            return await Task.Factory.StartNew(() =>
            DbSet.Where(fi =>
            fi.RPersonLocation.Any(ip =>
            ip.LocationId.Equals(resourceId)))
            .OrderBy(x => x.DisplayName).ToList());
        }

        public async Task<ICollection<Location>> GetByFileImage(string fileImageId)
        {
            return await Task.Factory.StartNew(() =>
             DbSet.Where(fi =>
           fi.RImagenFilesLocations.Any(ip =>
           ip.ImageFilesId.Equals(fileImageId)))
           .OrderBy(x => x.DisplayName).ToList());
        }

        public async Task<ICollection<Location>> GetByCalendarResource(string calendarResourceId)
        {
            return await Task.Factory.StartNew(() =>
             DbSet.Where(fi =>
             fi.RCalendarResourceLocations.Any(ip =>
            ip.CalendarResoureId.Equals(calendarResourceId)))
            .OrderBy(x => x.DisplayName).ToList());
        }
    }
}
