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
        /// <param name="db"></param>
        /// <param name="fs"></param>
        public static async Task SeedDb_Fs(CalDavContext db, IFileSystemManagement fs)
        {
            var frank = new User("Frank", "f.underwood@wh.org") { LastName = "Underwood" };
            var frankCollection = new CalendarCollection($"caldav/{frank.Email}/durtyplans/", "durtyPlans");
            frank.CalendarCollections.Add(frankCollection);
            db.Users.Add(frank);

            fs.AddUserFolder(frank.Email);
            fs.AddCalendarCollectionFolder(frank.Email, frankCollection.Name);

            var claire = new User("Claire", "c.underwood@wh.org") {LastName = "Underwood"};
            db.Users.Add(claire);
            fs.AddUserFolder(claire.Email);

            await db.SaveChangesAsync();
        }

        public static async Task RecreateDb(CalDavContext db, IFileSystemManagement fs)
        {
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
            fs.DestroyAll();
        }

        public static void DestroyAll(this IFileSystemManagement fs)
        {
            Directory.Delete(fs.Root, true);
        }
    }
}