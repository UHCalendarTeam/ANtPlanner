using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Contexts;
using DataLayer.Models.Entities;
using DataLayer.Models.Entities.ResourcesAndCollections;
using DataLayer.Models.Interfaces.Repositories;

namespace DataLayer.Models.Repositories
{
    public class ResourceRespository : CaldavEntitiesRepository<CalendarResource>,ICalendarResourceRepository
    {
        public ResourceRespository(CalDavContext context) : base(context)
        {
        }

        public override void InitializeStandardProperties(CalendarResource entity, string name)
        {
            entity.Properties.Add(new Property("getcontenttype",entity. NamespacesSimple["D"])
            {
                IsVisible = true,
                IsMutable = false,
                IsDestroyable = false,
                Value = $"<D:getcontenttype {entity.Namespaces["D"]}>text/calendar</D:getcontenttype>"
            });

            entity. Properties.Add(new Property("resourcetype", entity.NamespacesSimple["D"])
            { IsVisible = true, IsDestroyable = false, IsMutable = false, Value = $"<D:resourcetype {entity.Namespaces["D"]}/>" });

            entity.Properties.Add(new Property("displayname", entity.NamespacesSimple["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = true,
                Value = $"<D:displayname {entity.Namespaces["D"]}>Test</D:displayname>"
            });

            //TODO: Generar Etag en creacion.
            entity. Properties.Add(new Property("getetag", entity.NamespacesSimple["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $@"<D:getetag {entity.Namespaces["D"]}>""{Guid.NewGuid()}""</D:getetag>"
            });

            entity.Properties.Add(new Property("creationdate", entity.NamespacesSimple["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<D:creationdate {entity.Namespaces["D"]}>{DateTime.Now}</D:creationdate>"
            });

            entity. Properties.Add(new Property("getcontentlanguage", entity.NamespacesSimple["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = true,
                Value = $"<D:getcontentlanguage {entity.Namespaces["D"]}>en</D:getcontentlanguage>"
            });

            entity. Properties.Add(new Property("getcontentlength", entity.NamespacesSimple["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<D:getcontentlength {entity.Namespaces["D"]}>0</D:getcontentlength>"
            });

            entity. Properties.Add(new Property("getlastmodified", entity.NamespacesSimple["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<D:getlastmodified {entity.Namespaces["D"]}>{DateTime.Now}</D:getlastmodified>"
            });
            base.InitializeStandardProperties(entity, name);
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

        public async Task<ICollection<CalendarResource>> GetByPerson(string personId)
        {
            return await Task.Factory.StartNew(() =>DbSet.Where(cr=>
            cr.RCalendarResourcePersons.Any(rcr=>
            rcr.PersonId.Equals(personId))).ToList());
        }

        public async Task<ICollection<CalendarResource>> GetByResource(string resourceId)
        {
            return await Task.Factory.StartNew(() => DbSet.Where(cr =>
            cr.RResourceCalendarResources.Any(rcr =>
            rcr.ResourceId.Equals(resourceId))).ToList());
        }

        public async Task<ICollection<CalendarResource>> GetByFileImage(string fileImageId)
        {
            return await Task.Factory.StartNew(() => DbSet.Where(cr =>
            cr.RImageFilesResources.Any(rcr =>
            rcr.ImageFilesId.Equals(fileImageId))).ToList());
        }

        public async Task<ICollection<CalendarResource>> GetByLocation(string locationId)
        {
            return await Task.Factory.StartNew(() => DbSet.Where(cr =>
            cr.RCalendarResourceLocations.Any(rcr => 
            rcr.LocationId.Equals(locationId))).ToList());
        }

        public async Task<ICollection<CalendarResource>> GetByCalendarResource(string calendarResourceId)
        {
            return await Task.Factory.StartNew(() =>
            DbSet.Where(cr => cr.RelatedCalendarResources.Any(rcr =>
            rcr.Id.Equals(calendarResourceId))).ToList());
        }

        public CalendarResource FindUrl(string url)
        {
            return DbSet.FirstOrDefault(e => e.Url.Equals(url));
        }
    }
}