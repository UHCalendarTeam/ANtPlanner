using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Contexts;
using DataLayer.Models.Entities.OtherEnt.Resource;
using DataLayer.Models.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models.Repositories
{

    public class ResourceRepository : BaseRepository<Resource, string>, IResourceRepository
    {
        public ResourceRepository(CalDavContext context) : base(context)
        {
        }

        //todo:parece que no puedo hacer esta operacion.
        public async Task<ICollection<Resource>> FindByProperty(string property)
        {
            throw new NotImplementedException();
        }

        public  Resource FindWhithResourceType(string id)
        {
            return DbSet.Include(r => r.ResourceType).FirstOrDefault(r => r.Id == id);
        }

        public new Task<Resource> FindAsync(string id)
        {
            return DbSet.Include(r => r.ResourceType).FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<ICollection<Resource>> FindByTYpe(string type)
        {
            return await DbSet.Where(r => r.ResourceType.Type.Equals(type)).ToListAsync();
        }

        public async Task<ICollection<Resource>> GetByPerson(string personId)
        {
            return await Task.Factory.StartNew(() =>
              DbSet.Where(fi =>
             fi.RPersonResource.Any(ip =>
             ip.PersonId.Equals(personId))).ToList());
        }

        public async Task<ICollection<Resource>> GetByLocation(string locationId)
        {
            return await Task.Factory.StartNew(() =>
             DbSet.Where(fi =>
         fi.RLocationResources.Any(ip =>
             ip.LocationId.Equals(locationId))).ToList());
        }

        public async Task<ICollection<Resource>> GetByCalendarResource(string calendarResourceId)
        {
            return await Task.Factory.StartNew(() =>
             DbSet.Where(fi =>
             fi.ResourceCalendarResources.Any(ip =>
             ip.CalendarResourceId.Equals(calendarResourceId))).ToList());
        }
    }
}
