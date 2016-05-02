using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.ACL;
using DataLayer.Models.Entities;
using Microsoft.Data.Entity;

namespace DataLayer.Repositories.Implementations
{
    public class PrincipalRepository : IRepository<Principal, string>
    {
        private readonly CalDavContext _context;

        public PrincipalRepository(CalDavContext context)
        {
            _context = context;
        }



        public IList<Principal> GetAll()
        {
            return _context.Principals.ToList();
        }

        public async Task<Principal> Get(string url)
        {
            return await _context.Principals.FirstOrDefaultAsync(p => p.PrincipalURL == url);
        }

        public void Add(Principal entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(Principal entity)
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

        public IList<Property> GetAllProperties(string url)
        {
            throw new NotImplementedException();
        }

        public IList<Property> GetProperties(string url, List<KeyValuePair<string, string>> propertiesNameandNs)
        {
            throw new NotImplementedException();
        }

        public IList<KeyValuePair<string, string>> GetAllPropname(string url)
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

        Task<IList<Principal>> IRepository<Principal, string>.GetAll()
        {
            throw new NotImplementedException();
        }

        Task<IList<Property>> IRepository<Principal, string>.GetAllProperties(string url)
        {
            throw new NotImplementedException();
        }

        Task<IList<Property>> IRepository<Principal, string>.GetProperties(string url, List<KeyValuePair<string, string>> propertiesNameandNs)
        {
            throw new NotImplementedException();
        }

        Task<IList<KeyValuePair<string, string>>> IRepository<Principal, string>.GetAllPropname(string url)
        {
            throw new NotImplementedException();
        }
    }
}
