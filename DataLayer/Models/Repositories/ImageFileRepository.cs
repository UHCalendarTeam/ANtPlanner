using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Contexts;
using DataLayer.Models.Entities.OtherEnt;
using DataLayer.Models.Interfaces.Repositories;

namespace DataLayer.Models.Repositories
{
    public class ImageFileFileRepository : BaseRepository<FileImage, string>, IImageFileRepository
    {
        public ImageFileFileRepository(CalDavContext context) : base(context)
        {
        }

        public async Task<ICollection<FileImage>> GetByPerson(string personId)
        {
            return await Task.Factory.StartNew(() =>
            DbSet.Where(fi=>
            fi.RImagensFilesPersons.Any(ip=>
            ip.PersonId.Equals(personId))).OrderBy(x => x.DisplayName).ToList());
        }

        public async Task<ICollection<FileImage>> GetByResource(string resourceId)
        {
            return await Task.Factory.StartNew(() => DbSet.Where(fi => 
            fi.RImagensFilesResources.Any(ip =>
            ip.ResourceId.Equals(resourceId))).OrderBy(x => x.DisplayName).ToList());
        }

        public async Task<ICollection<FileImage>> GetByLocation(string locationId)
        {
            return await Task.Factory.StartNew(() => 
            DbSet.Where(fi => 
            fi.RImagenFilesLocations.Any(ip =>
            ip.LocationId.Equals(locationId))).OrderBy(x => x.DisplayName).ToList());
        }

        public async Task<ICollection<FileImage>> GetByCalendarResource(string calendarResourceId)
        {
            return await Task.Factory.StartNew(() =>
            DbSet.Where(fi => 
            fi.RCalendarResourcesImagenFiles.Any(ip =>
            ip.CalendarResourceId.Equals(calendarResourceId))).OrderBy(x =>x.DisplayName).ToList());
        }
    }
}
