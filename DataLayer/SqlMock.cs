using System.IO;
using System.Threading.Tasks;
using DataLayer.Models.Entities;
using Microsoft.AspNet.Http;

namespace DataLayer
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public static class SqlMock
    {
        /// <summary>
        /// This is method is used to seed the database with some fictional values.
        /// </summary>
        public static void SeedDb_Fs()
        {
            using (var db = new CalDavContext())
            {
                var fs = new FileSystemManagement();
                var frank = new User("Frank", "f.underwood@wh.org") { LastName = "Underwood" };
                var frankCollection = new CalendarCollection($"caldav/{frank.Email}/durtyplans/", "durtyplans");
                var assesinationEvent = new CalendarResource("api/v1/caldav/f.underwood@wh.org/durtyplans/russodies", "russodies");
                frankCollection.CalendarResources.Add(assesinationEvent);
                frank.CalendarCollections.Add(frankCollection);
                db.Users.Add(frank);

                fs.AddUserFolder(frank.Email);
                fs.AddCalendarCollectionFolder(frank.Email, frankCollection.Name);

                var claire = new User("Claire", "c.underwood@wh.org") { LastName = "Underwood" };
                db.Users.Add(claire);
                fs.AddUserFolder(claire.Email);

                db.SaveChanges();
            }

        }

        public static void RecreateDb()
        {
            using (var db = new CalDavContext())
            {
                var fs = new FileSystemManagement();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                db.SaveChanges();
                fs.DestroyAll();
            }
        }

        public static void DestroyAll(this IFileSystemManagement fs)
        {
            if (Directory.Exists(fs.Root))
                Directory.Delete(fs.Root, true);
        }
    }
}