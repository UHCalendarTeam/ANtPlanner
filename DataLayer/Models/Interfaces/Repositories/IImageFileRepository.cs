using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer.Models.Entities.OtherEnt;

namespace DataLayer.Models.Interfaces.Repositories
{
   public interface IImageFileRepository:IRepository<FileImage,string>
    {

        Task<ICollection<FileImage>> GetByPerson(string personId);

        Task<ICollection<FileImage>> GetByResource(string resourceId);

        Task<ICollection<FileImage>> GetByLocation(string locationId);

        Task<ICollection<FileImage>> GetByCalendarResource(string calendarResourceId);
    }
}
