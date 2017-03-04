using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Contexts;
using DataLayer.Models.Entities;
using DataLayer.Models.Entities.ResourcesAndCollections;
using DataLayer.Models.Interfaces;
using DataLayer.Models.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models.Repositories
{
    public class PropertyContainerRepository<TEnt, TKey> : BaseRepository<TEnt, TKey>, IPropertyContainerRepository<TEnt, TKey> where TEnt : class, IPropertyContainer, IEntity<TKey>
    {
        public PropertyContainerRepository(CalDavContext context) : base(context)
        {
        }

        public async Task<IList<Property>> GetAllProperties(TKey url)
        {
            var collection = await FindWithPropertiesAsync(url);

            return collection?.Properties.Where(p => p.IsVisible).ToList();
        }

        public virtual TEnt FindWithProperties(TKey key)
        {
            return DbSet.Include(p => p.Properties).FirstOrDefault(c => c.Id.Equals(key));
        }

        public virtual Task<TEnt> FindWithPropertiesAsync(TKey key)
        {
            return DbSet.Include(p => p.Properties).FirstOrDefaultAsync(c => c.Id.Equals(key));
        }


        public async Task<Property> GetProperty(TKey id, KeyValuePair<string, string> propertyNameandNs)
        {
            var collection = await FindWithPropertiesAsync(id);

            try
            {
                var property = string.IsNullOrEmpty(propertyNameandNs.Value)
                ? collection?.Properties.FirstOrDefault(p => p.Name == propertyNameandNs.Key)
                : collection?.Properties.FirstOrDefault(
                    p => p.Name == propertyNameandNs.Key && p.Namespace == propertyNameandNs.Value);
                return property;
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }

            return null;
        }


        public async Task<IList<KeyValuePair<string, string>>> GetAllPropname(TKey key)
        {
            var collection = await FindWithPropertiesAsync(key);
            return
                collection?.Properties.ToList()
                    .Select(p => new KeyValuePair<string, string>(p.Name, p.Namespace))
                    .ToList();
        }

        public async Task<bool> RemoveProperty(TKey key, KeyValuePair<string, string> propertyNameNs, Stack<string> errorStack)
        {
            var collection = await FindWithPropertiesAsync(key);
            var property = string.IsNullOrEmpty(propertyNameNs.Value)
                ? collection?.Properties.FirstOrDefault(p => p.Name == propertyNameNs.Key)
                : collection?.Properties.FirstOrDefault(
                    p => p.Name == propertyNameNs.Key && p.Namespace == propertyNameNs.Value);
            if (property == null)
                return true;
            if (!property.IsDestroyable)
            {
                errorStack.Push("HTTP/1.1 403 Forbidden");
                return false;
            }
            collection?.Properties.Remove(property);
            await SaveChangesAsync();
            //await SaveChanges();
            return true;
        }

        public async Task<bool> CreateOrModifyProperty(TKey key, string propName, string propNs, string propValue, Stack<string> errorStack,
            bool adminPrivilege)
        {
            var collection = await FindWithPropertiesAsync(key);
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
                await Context.SaveChangesAsync();
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
            await SaveChangesAsync();
            //await SaveChanges();
            return true;
        }
    }
}
