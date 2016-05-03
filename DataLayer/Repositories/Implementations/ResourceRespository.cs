using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.Entities;
using Microsoft.Data.Entity;

namespace DataLayer.Repositories
{
    public class ResourceRespository : IRepository<CalendarResource, string>
    {
        private readonly CalDavContext _context;

        public ResourceRespository(CalDavContext context)
        {
            _context = context;
        }
        public async Task<IList<CalendarResource>> GetAll()
        {
            return await Task.FromResult(_context.CalendarResources.ToList());
        }

        public async Task<CalendarResource> Get(string url)
        {
            return _context.CalendarResources.Include(r => r.Properties)
                 .FirstOrDefault(r => r.Href == url);
        }

        public async Task Add(CalendarResource entity)
        {
            _context.CalendarResources.Add(entity);
            //await _context.SaveChangesAsync();
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
            return await Task.FromResult(_context.CalendarResources.Count());
        }

        public async Task<bool> Exist(string url)
        {
            return await Task.FromResult(_context.CalendarResources.Any(r => r.Href == url));
        }

        public async Task<IList<Property>> GetAllProperties(string url)
        {
            var resource =await Get(url);
            return await Task.FromResult(resource?.Properties.ToList());
            
        }

        public async Task<Property> GetProperty(string url, KeyValuePair<string, string> propertyNameandNs)
        {
            var resource = await Get(url);
            Property property;

            if (resource == null)
                return null;

            if (string.IsNullOrEmpty(propertyNameandNs.Value))
                property = resource.Properties.FirstOrDefault(
                    prop => prop.Name == propertyNameandNs.Key && prop.Namespace == propertyNameandNs.Value);
            else
                property = resource.Properties.FirstOrDefault(
                    prop => prop.Name == propertyNameandNs.Key);
            return property;
        }

        public async Task<IList<KeyValuePair<string, string>>> GetAllPropname(string url)
        {
            var resource = await Get(url);
            return
              await Task.FromResult(resource.Properties.ToList().Select(prop => new KeyValuePair<string, string>(prop.Name, prop.Namespace))
                    .ToList());
        }

        public async Task<bool> RemoveProperty(string url, KeyValuePair<string, string> propertyNameNs, Stack<string> errorStack)
        {
            var resource =await Get(url);
            var property =await GetProperty(url, propertyNameNs);

            if (property == null)
                return await Task.FromResult(false);
            if (property.IsDestroyable)
            {
                resource.Properties.Remove(property);
            }
            return await Task.FromResult(true);
        }

        public async Task<bool> CreateOrModifyProperty(string url, string propName, string propNs, string propValue, Stack<string> errorStack,
            bool adminPrivilege)
        {
            var resource =await Get(url);



            var propperty =await GetProperty(url, new KeyValuePair<string, string>(propName, propNs));

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

            return await Task.FromResult(true);
        }

        public async Task<bool> CreatePropertyForResource(CalendarResource resource, string propName, string propNs,
            string propValue, Stack<string> errorStack,
            bool adminPrivilege)
        {

            var prop = new Property(propName, propNs)
            {
                Value = propValue
            };
            resource.Properties.Add(prop);


            return await Task.FromResult(true);
        }

        public Task<bool> ExistByStringIs(string identifier)
        {
            throw new NotImplementedException();
        }

        public async Task<int> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync();
        }

       
    }
}
