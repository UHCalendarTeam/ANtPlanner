using System.Linq;
using System.Threading.Tasks;
using DataLayer.Contexts;
using DataLayer.Models.Entities;
using DataLayer.Models.Entities.ACL;
using DataLayer.Models.Entities.ResourcesAndCollections;
using DataLayer.Models.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models.Repositories
{
    public class HomeRepository : CaldavEntitiesRepository<CalendarHome>,IHomeRepository
    {
        public HomeRepository(CalDavContext context) : base(context)
        {
            
        }

        public  CalendarHome FindWihtPropertiesAndCalendarCollections(string url)
        {
            return Context.CalendarHomeCollections.Include(p => p.Properties).
              Include(r => r.CalendarCollections).ThenInclude(rp => rp.Properties).FirstOrDefault(c => c.Url == url);
        }

        public CalendarHome FindWihtProperties(string url)
        {
            return Context.CalendarHomeCollections.Include(p => p.Properties).FirstOrDefault(c => c.Url == url);
        }

        public  CalendarHome FindWihtCalendarCollections(string url)
        {
            return Context.CalendarHomeCollections.
              Include(r => r.CalendarCollections).ThenInclude(rp => rp.Properties).FirstOrDefault(c => c.Url == url);
        }

        public new async Task<CalendarHome> FindAsync(string url)
        {
            return await Context.CalendarHomeCollections.Include(p => p.Properties).
                Include(r => r.CalendarCollections)
                .ThenInclude(rp => rp.Properties)
                .FirstOrDefaultAsync(c => c.Url == url);
        }

        public override void InitializeStandardProperties(CalendarHome entity, string name)
        {
            entity.Properties.Add(new Property("resourcetype", SystemProperties.NamespacesValues["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value =
                   $"<D:resourcetype {SystemProperties.Namespaces["D"]}><D:collection/></D:resourcetype>"
            });
            base.InitializeStandardProperties(entity, name);
        }

        public static CalendarHome CreateCalendarHome(Principal owner)
         {
            //check if the user is an admin user.
            //if it is the first admin user then create the public 
            //calendars
            var created = SystemProperties.PublicCalendarCreated;
            var adminUser = owner.PrincipalStringIdentifier.EndsWith("@admin.uh.cu");

            var fsm = new FileManagement();
            var defaultCalName = "DefaultCalendar";

            var defaultCalHomeName = adminUser ? "PublicCollections" : "HomeCollection";
            var ownerProp = PropertyCreation.CreateProperty("owner", "D", $"<D:href>{owner.PrincipalUrl}</D:href>",
               false, false);

            var aclProperty = adminUser ? PropertyCreation.CreateAclPropertyForGroupCollections(owner.PrincipalUrl) : PropertyCreation.CreateAclPropertyForUserCollections(owner.PrincipalUrl);

            string calHomeUrl;
            //if the user is admin then the collection home is public so the URL change
            if (adminUser)
                calHomeUrl = SystemProperties.PublicCalendarHomeUrl;
            else
                calHomeUrl = $"{SystemProperties._userCollectionUrl}{owner.PrincipalStringIdentifier}/";


            var calHome = new CalendarHome(
                calHomeUrl, defaultCalHomeName, ownerProp, aclProperty);

            fsm.CreateFolder(calHome.Url);
            ownerProp = PropertyCreation.CreateProperty("owner", "D", $"<D:href>{owner.PrincipalUrl}</D:href>",
              false, false);

            aclProperty = adminUser ? PropertyCreation.CreateAclPropertyForGroupCollections(owner.PrincipalUrl) : PropertyCreation.CreateAclPropertyForUserCollections(owner.PrincipalUrl);

            //create the initial calendar collection for the user.
            var initCollection =
                new CalendarCollection(
                    $"{calHome.Url}{defaultCalName}/",
                    defaultCalName, ownerProp, aclProperty)
                {
                   PrincipalId= owner.Id,
                    CalendarHomeId = calHome.Id
                                    };


            //if the principal is admin then create the public calendars
            if (!created && adminUser)
                CreatePublicCollections(calHome, owner, aclProperty, ownerProp);


            //add the calendar collection to the calHome
            //if the principal is not admin
            if (!adminUser)
            {
                fsm.CreateFolder(initCollection.Url);
                calHome.CalendarCollections.Add(initCollection);
                //owner.CalendarCollections.Add(initCollection);
            }

            return calHome;
        }

        public static void CreatePublicCollections(CalendarHome publicCalendar, Principal owner, params Property[] properties)
        {
            var fsm = new FileManagement();
            foreach (var calName in SystemProperties.PublicCalendarNames)
            {
                var ownerProp = PropertyCreation.CreateProperty("owner", "D", $"<D:href>{owner.PrincipalUrl}</D:href>",
              false, false);

                var aclProperty = PropertyCreation.CreateAclPropertyForGroupCollections(owner.PrincipalUrl);
                var publicCollection =
                new CalendarCollection(
                    $"{SystemProperties.PublicCalendarHomeUrl}{calName}/", calName, ownerProp, aclProperty)
                {
                    PrincipalId = owner.Id,
                    CalendarHomeId = publicCalendar.Id
                };

                fsm.CreateFolder(publicCollection.Url);
                publicCalendar.CalendarCollections.Add(publicCollection);
                //owner.CalendarCollections.Add(publicCollection);
            }
        }

        public CalendarHome FindUrl(string url)
        {
            return DbSet.FirstOrDefault(e => e.Url.Equals(url));
        }
    }
}
