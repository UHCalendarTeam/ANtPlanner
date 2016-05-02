using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.Entities;
using Microsoft.Data.Entity;

namespace DataLayer.Repositories
{
    public class CollectionRepository : IRepository<CalendarCollection, string>, IDisposable
    {
        private readonly CalDavContext _context;

        public CollectionRepository(CalDavContext context)
        {
            _context = context;
        }


        public async Task<IList<CalendarCollection>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<CalendarCollection> Get(string url)
        {
            return await _context.CalendarCollections.Include(p => p.Properties).
                Include(r => r.CalendarResources).ThenInclude(rp => rp.Properties).FirstOrDefaultAsync(c => c.Url == url);
        }

        public async Task Add(CalendarCollection entity)
        {
            _context.CalendarCollections.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Remove(CalendarCollection entity)
        {
            _context.CalendarCollections.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Remove(string url)
        {
            var collection = await Get(url);
            await Remove(collection);
        }

        public async Task<int> Count()
        {
            return await _context.CalendarCollections.CountAsync();
        }

        public async Task<bool> Exist(string url)
        {
            return await _context.CalendarCollections.AnyAsync(c => c.Url == url);
        }

        public IList<Property> GetAllProperties(string url)
        {
            var collection = Get(url).Result;

            return collection?.Properties.Where(p => p.IsVisible).ToList();
        }

        public Property GetProperty(string url, KeyValuePair<string, string> propertyNameandNs)
        {
            var collection = Get(url).Result;

            var property = string.IsNullOrEmpty(propertyNameandNs.Value) ? collection?.Properties.FirstOrDefault(p => p.Name == propertyNameandNs.Key) : collection?.Properties.FirstOrDefault(p => p.Name == propertyNameandNs.Key && p.Namespace == propertyNameandNs.Value);

            return property;
        }

        public IList<KeyValuePair<string, string>> GetAllPropname(string url)
        {
            var collection = Get(url).Result;
            return collection?.Properties.Select(p => new KeyValuePair<string, string>(p.Name, p.Namespace)).ToList();
        }

        public  bool RemoveProperty(string url, KeyValuePair<string, string> propertyNameNs, Stack<string> errorStack)
        {
            var collection =  Get(url).Result;
            var property = string.IsNullOrEmpty(propertyNameNs.Value) ? collection?.Properties.FirstOrDefault(p => p.Name == propertyNameNs.Key) : collection?.Properties.FirstOrDefault(p => p.Name == propertyNameNs.Key && p.Namespace == propertyNameNs.Value);
            if (property == null)
                return true;
            if (!property.IsDestroyable)
            {
                errorStack.Push("HTTP/1.1 403 Forbidden");
                return false;
            }
            collection?.Properties.Remove(property);
            return true;
        }

        public  bool CreateOrModifyProperty(string url, string propName, string propNs, string propValue, Stack<string> errorStack, bool adminPrivilege)
        {
            var collection =  Get(url).Result;
            //get the property
            var property =
                collection.Properties
                    .FirstOrDefault(prop => prop.Name == propName && prop.Namespace == propNs);
            //if the property did not exist it is created.
            if (property == null)
            {
                collection.Properties.Add(new Property
                {
                    Name = propName,
                    Namespace = propNs,
                    IsDestroyable = true,
                    IsVisible = false,
                    IsMutable = true,
                    Value = propValue
                });
                return true;
            }
            //if this property belongs to the fix system properties, it can not be changed. Only the server can.
            if (!property.IsMutable && !adminPrivilege)
            {
                errorStack.Push("HTTP/1.1 403 Forbidden");
                return false;
            }

            //if all previous conditions don't pass then the value of the property is changed.
            property.Value = propValue;
            return true;
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
