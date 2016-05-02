using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.Entities;
using Microsoft.Data.Entity;

namespace DataLayer.Repositories
{
    public class ResourceRespository : IRepository<CalendarResource, string>, IDisposable
    {
        private readonly CalDavContext _context;

        public ResourceRespository(CalDavContext context)
        {
            _context = context;
        }
        public async Task<IList<CalendarResource>> GetAll()
        {
            return await _context.CalendarResources.ToListAsync();
        }

        public async Task<CalendarResource> Get(string url)
        {
            return await _context.CalendarResources.Include(r => r.Properties)
                 .FirstOrDefaultAsync(r => r.Href == url);
        }

        public async Task Add(CalendarResource entity)
        {
            _context.CalendarResources.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Remove(CalendarResource entity)
        {
            if (entity == null)
                return;
            _context.CalendarResources.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Remove(string url)
        {
            var resource = _context.CalendarResources.FirstOrDefaultAsync(r => r.Href == url).Result;
            await Remove(resource);
        }

        public async Task<int> Count()
        {
            return await _context.CalendarResources.CountAsync();
        }

        public async Task<bool> Exist(string url)
        {
            return await _context.CalendarResources.AnyAsync(r => r.Href == url);
        }

        public IList<Property> GetAllProperties(string url)
        {
            var resource = Get(url).Result;
            return resource?.Properties.ToList();
        }

        public Property GetProperty(string url, KeyValuePair<string, string> propertyNameandNs)
        {
            var resource = Get(url).Result;
            Property property;

            if (string.IsNullOrEmpty(propertyNameandNs.Value))
                property = resource.Properties.FirstOrDefault(
                    prop => prop.Name == propertyNameandNs.Key && prop.Namespace == propertyNameandNs.Value);
            else
                property = resource.Properties.FirstOrDefault(
                    prop => prop.Name == propertyNameandNs.Key);
            return property;
        }

        public IList<KeyValuePair<string, string>> GetAllPropname(string url)
        {
            var resource = Get(url).Result;
            return
                resource.Properties.Select(prop => new KeyValuePair<string, string>(prop.Name, prop.Namespace))
                    .ToList();
        }

        public bool RemoveProperty(string url, KeyValuePair<string, string> propertyNameNs, Stack<string> errorStack)
        {
            var resource = Get(url).Result;
            var property = GetProperty(url, propertyNameNs);

            if (property == null)
                return false;
            if (property.IsDestroyable)
            {
                resource.Properties.Remove(property);
            }
            return true;
        }

        public bool CreateOrModifyProperty(string url, string propName, string propNs, string propValue, Stack<string> errorStack,
            bool adminPrivilege)
        {
            var resource = Get(url).Result;

            var propperty = GetProperty(url, new KeyValuePair<string, string>(propName, propNs));

            //if the property is null then create it
            if (propperty == null)
            {
                var prop = new Property(propName, propNs)
                {
                    Value = propValue
                };
                resource.Properties.Add(prop);
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
        
        public Task<bool> ExistByStringIs(string identifier)
        {
            throw new NotImplementedException();
        }

        public async Task<int> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
