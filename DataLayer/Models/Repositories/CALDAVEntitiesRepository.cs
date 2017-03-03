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
    public class CaldavEntitiesRepository<TEnt> : PropertyContainerRepository<TEnt, string> where TEnt : AbstractCalendar
    {
        public CaldavEntitiesRepository(CalDavContext context) : base(context)
        {
        }

        public override void Add(TEnt entity)
        {
            InitializeStandardProperties(entity, entity.Name);
            base.Add(entity);
        }

        public override void AddRange(IEnumerable<TEnt> entities)
        {
            foreach (var entity in entities)
            {
                Add(entity);
            }
        }

        public virtual void InitializeStandardProperties(TEnt entity, string name)
        {
            entity.Properties.Add(new Property("calendar-timezone", SystemProperties.NamespacesValues["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<C:calendar-timezone {SystemProperties.Namespaces["C"]}>LaHabana/Cuba</C:calendar-timezone>"
            });

            entity.Properties.Add(new Property("getcontentlength", SystemProperties.NamespacesValues["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<D:getcontentlength {SystemProperties.Namespaces["D"]}>0</D:getcontentlength>"
            });
            entity.Properties.Add(new Property("displayname", SystemProperties.NamespacesValues["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = true,
                Value =
                                string.IsNullOrEmpty(name)
                                    ? null
                                    : $"<D:displayname {SystemProperties.Namespaces["D"]}>{name}</D:displayname>"
            });

            entity.Properties.Add(new Property("creationdate", SystemProperties.NamespacesValues["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = true,
                Value = $"<D:creationdate {SystemProperties.Namespaces["D"]}>{DateTime.Now}</D:creationdate>"
            });

            entity.Properties.Add(new Property("getctag", SystemProperties.NamespacesValues["CS"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $@"<CS:getctag {SystemProperties.Namespaces["CS"]} >{Guid.NewGuid()}</CS:getctag>"
            });
            //todo: remember, the inheritance class call base ending the override.
            entity.Properties.Add(PropertyCreation.CreateSupportedPrivilegeSetForResources());
        }

        public override TEnt FindWithProperties(string key)
        {
            return DbSet.Include(p => p.Properties).FirstOrDefault(c => c.Url.Equals(key));
        }

        public override Task<TEnt> FindWithPropertiesAsync(string key)
        {
            return DbSet.Include(p => p.Properties).FirstOrDefaultAsync(c => c.Url.Equals(key));
        }
    }
}
