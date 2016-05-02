using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.Entities;

namespace DataLayer.Repositories.Implementations
{
    public class CollectionRepository:IRepository<CalendarCollection, string>
    {
        private readonly CalDavContext _context;

        public CollectionRepository(CalDavContext context)
        {
            _context = context;
        }

        public Task<IList<CalendarCollection>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<CalendarCollection> Get(string url)
        {
            throw new NotImplementedException();
        }

        public void Add(CalendarCollection entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(CalendarCollection entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(string url)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public bool Exist(string url)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Property>> GetAllProperties(string url)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Property>> GetProperties(string url, List<KeyValuePair<string, string>> propertiesNameandNs)
        {
            throw new NotImplementedException();
        }

        public Task<IList<KeyValuePair<string, string>>> GetAllPropname(string url)
        {
            throw new NotImplementedException();
        }

        public bool RemoveProperty(string url, KeyValuePair<string, string> propertyNameNs, Stack<string> errorStack)
        {
            throw new NotImplementedException();
        }

        public bool CreateOrModifyProperty(string url, string propName, string propNs, string propValue, Stack<string> errorStack,
            bool adminPrivilege)
        {
            throw new NotImplementedException();
        }
    }
}
