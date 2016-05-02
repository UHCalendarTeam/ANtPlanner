using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.ACL;
using DataLayer.Models.Entities;
using Microsoft.Data.Entity;

namespace DataLayer.Repositories
{
    public class PrincipalRepository : IRepository<Principal, string>, IDisposable
    {
        private readonly CalDavContext _context;

        public PrincipalRepository(CalDavContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            _context.Dispose();
        }


        public async Task<IList<Principal>> GetAll()
        {
            return await _context.Principals.ToListAsync();
        }

        public async Task<Principal> Get(string url)
        {
            return await _context.Principals.Include(p => p.Properties)
                .FirstOrDefaultAsync(p => p.PrincipalURL == url);
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

        public async Task Remove(string url)
        {
            var principal = _context.Principals.FirstOrDefaultAsync(p => p.PrincipalURL == url);
            await Remove(principal.Result);
        }

        public Task<int> Count()
        {
            return _context.Principals.CountAsync();
        }

        public async Task<bool> Exist(string url)
        {
            return await _context.Principals.AnyAsync(p => p.PrincipalURL == url);
        }

        public IList<Property> GetAllProperties(string url)
        {
            var principal = Get(url);

            return principal.Result?.Properties.Where(x => x.IsVisible).ToList();
        }

        public Property GetProperty(string url, KeyValuePair<string, string> propertyNameandNs)
        {
            var principal = Get(url).Result;
            Property property;

            if (string.IsNullOrEmpty(propertyNameandNs.Value))
                property = principal.Properties.FirstOrDefault(
                    prop => prop.Name == propertyNameandNs.Key && prop.Namespace == propertyNameandNs.Value);
            else
                property = principal.Properties.FirstOrDefault(
                    prop => prop.Name == propertyNameandNs.Key);
            return property;
        }

        public IList<KeyValuePair<string, string>> GetAllPropname(string url)
        {
            var principal = Get(url).Result;
            return
                principal.Properties.Select(prop => new KeyValuePair<string, string>(prop.Name, prop.Namespace))
                    .ToList();
        }

        public bool RemoveProperty(string url, KeyValuePair<string, string> propertyNameNs,
        {
            var principal = Get(url).Result;
            var property =
                principal.Properties.FirstOrDefault(
                    prop => prop.Name == propertyNameNs.Key && prop.Namespace == propertyNameNs.Value);

            if (property == null)
                return false;
            if (property.IsDestroyable)
            {
                principal.Properties.Remove(property);
            }
            return true;

        }

        public   bool CreateOrModifyProperty(string url, string propName, string propNs, string propValue,
        {
            var principal = Get(url).Result;
            var propperty =
                principal.Properties.FirstOrDefault(prop => prop.Name == propName && prop.Namespace == propNs);

            //if the property is null then create it
            if (propperty == null)
            {
                var prop = new Property(propName, propNs)
                {
                    Value = propValue
                };
                principal.Properties.Add(prop);
            }
            else
            {
                if (propperty.IsMutable || adminPrivilege)
                {
                    propperty.Value = propValue;
                }
            }
            return true;
        }

        Task<IList<Principal>> IRepository<Principal, string>.GetAll()
        {
            throw new NotImplementedException();
        }
       
    }
}
