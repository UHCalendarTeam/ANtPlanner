using System.Linq;
using System.Threading.Tasks;
using DataLayer.Contexts;
using DataLayer.Models.Entities;
using DataLayer.Models.Entities.ResourcesAndCollections;
using DataLayer.Models.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models.Repositories
{
    public class CalendarCollectionRepository : CaldavEntitiesRepository<CalendarCollection>,ICalendarCollectionRepository
    {
        public CalendarCollectionRepository(CalDavContext context):base(context)
        {
        }

        public CalendarCollection FindwithCalendarResource(string url)
        {
            return Context.CalendarCollections.
                Include(r => r.CalendarResources).ThenInclude(rp => rp.Properties).FirstOrDefault(c => c.Url == url);
        }

        public CalendarCollection FindwithPropeties(string url)
        {
            return Context.CalendarCollections.Include(p => p.Properties).FirstOrDefault(c => c.Url == url);
        }

        public CalendarCollection FindwithPropetiesAndCalendarResource(string url)
        {
            return Context.CalendarCollections.Include(p => p.Properties).
                Include(r => r.CalendarResources).ThenInclude(rp => rp.Properties).FirstOrDefault(c => c.Url == url);
        }

        public new  async Task<CalendarCollection> FindAsync(string url)
        {
            return await Context.CalendarCollections.Include(p => p.Properties).
                Include(r => r.CalendarResources)
                .ThenInclude(rp => rp.Properties)
                .FirstOrDefaultAsync(c => c.Url == url);
        }

        public override void InitializeStandardProperties(CalendarCollection entity, string name)
        {
            entity.Properties.Add(new Property("max-resource-size", SystemProperties.NamespacesValues["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<C:max-resource-size {SystemProperties.Namespaces["C"]}>102400</C:max-resource-size>"
            });


            entity.Properties.Add(new Property("min-date-time", SystemProperties.NamespacesValues["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value =
                    $"<C:min-date-time {SystemProperties.Namespaces["C"]}>{SystemProperties.MinDateTime()}</C:min-date-time>"
            });

            entity.Properties.Add(new Property("max-date-time", SystemProperties.NamespacesValues["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value =
                    $"<C:max-date-time {SystemProperties.Namespaces["C"]}>{SystemProperties.MaxDateTime()}</C:max-date-time>"
            });


            entity.Properties.Add(new Property("max-instances", SystemProperties.NamespacesValues["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<C:max-instances {SystemProperties.Namespaces["C"]}>10</C:max-instances>"
            });
            entity.Properties.Add(new Property("supported-calendar-component-set", SystemProperties.NamespacesValues["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = true,
                Value =
                    $@"<C:supported-calendar-component-set {SystemProperties.Namespaces["C"]
                        }><C:comp name=""VEVENT""/><C:comp name=""VTODO""/></C:supported-calendar-component-set>"
            });

            entity.Properties.Add(new Property("supported-calendar-data", SystemProperties.NamespacesValues["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value =
                    $@"<C:supported-calendar-data {SystemProperties.Namespaces["C"]
                        }><C:comp name=""VEVENT""/><C:comp name=""VTODO""/></C:supported-calendar-data>"
            });

            entity.Properties.Add(new Property("calendar-description", SystemProperties.NamespacesValues["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = true,
                Value =
                    $"<C:calendar-description {SystemProperties.Namespaces["C"]}>No Description Available</C:calendar-description>"
            });
            entity.Properties.Add(new Property("resourcetype", SystemProperties.NamespacesValues["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value =
                    $"<D:resourcetype {SystemProperties.Namespaces["D"]}><D:collection/><C:calendar xmlns:C=\"urn:ietf:params:xml:ns:caldav\"/></D:resourcetype>"
            });
            base.InitializeStandardProperties(entity, name);
        }
    }
}