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



        public async Task<IList<Principal>> GetAll()
        {
            return await _context.Principals.ToListAsync();
        }

        public async Task<Principal> Get(string url)
        {
            return await _context.Principals.FirstOrDefaultAsync(p => p.PrincipalURL == url);
        }

         public async Task Add(Principal entity)
        {
           _context.Principals.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Remove(Principal entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();

        }

        public Task Remove(string url)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Exist(string url)
        {
            return await _context.Principals.AnyAsync(p => p.PrincipalURL == url);
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

        public async Task RemoveProperty(string url, KeyValuePair<string, string> propertyNameNs, Stack<string> errorStack)
        {
            throw new NotImplementedException();
        }

        public Task CreateOrModifyProperty(string url, string propName, string propNs, string propValue, Stack<string> errorStack,
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
