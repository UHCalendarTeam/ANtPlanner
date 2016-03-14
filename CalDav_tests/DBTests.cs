using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.Models;
using Microsoft.Data.Entity;
using Xunit;

namespace CalDav_tests
{
    /// <summary>
    /// This class contains the test for the work
    /// with the DB.
    /// </summary>
    public class DBTests
    {
        [Fact]
        public void UnitTest1()
        {
            var optionsBuilder = new DbContextOptionsBuilder<CalDavContext>();

            // This is the magic line
            optionsBuilder.UseInMemoryDatabase();

            var db = new CalDavContext(optionsBuilder.Options);
            var user = new User()
            {
                Email = "foo@gmail.com",
                LastName = "Doo",
                FirstName = "John",
                CalendarCollections = new List<CalendarCollection>() { }
            };
            var resources = new List<CalendarResource>()
            {
                new CalendarResource()
                {
                    DtStart = DateTime.Now,
                    DtEnd = DateTime.Now,
                    FileName = "test.ics",
                    Recurrence = "test",
                    User = user

                }
            };
            var collection = new List<CalendarCollection>()
            {
                new CalendarCollection()
                {
                    CalendarDescription = "Foo description",
                    Name = "Foo collection",
                    User = user,
                    CalendarResources = resources/*,
                    SupportedCalendarComponentSet = new List<string>()*/,
                    MaxIntences = 1,
                    CalendarTimeZone = "Timezone",
                    ResourceType = new List<string>(),
                    MaxDateTime = DateTime.Now,
                    MinDateTime = DateTime.MinValue,
                    DisplayName = "Display name",
                    Url = "url"
                    
                }
            };
            user.CalendarCollections = collection;
            user.Resources = resources;
            db.Users.Add(user);

            var userResult = db.Users.First(x => x.FirstName == "John");
            Assert.NotNull(user);
            var a = 4;
        }
    }
}
