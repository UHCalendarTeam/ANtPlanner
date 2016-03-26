using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.Models;
using CalDAV.Utils.XML_Processors;
using Microsoft.Data.Entity;
using TreeForXml;
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
                    //TODO: Adriano ver esto
                    //DtStart = DateTime.Now,
                    //DtEnd = DateTime.Now,
                    FileName = "test.ics",
                    //Recurrence = "test",
                    User = user,
                    Getetag = "12345",
                    Creationdate = DateTime.Now.ToString(),


                }
            };
            var collection = new List<CalendarCollection>()
            {
                new CalendarCollection()
                {
                    Calendardescription = "Foo description",
                    Name = "Foo collection",
                    User = user,
                    Calendarresources = resources/*,
                    SupportedCalendarComponentSet = new List<string>()*/,
                    //ResourceType = new List<string>(),

                    //TODO: Adriano ver esto ahora es xml hecho string
                    //ResourceType = new XmlTreeStructure("resourcetype", "DAV"),
                    Displayname = "Display name",
                    Url = "url"
                    
                }
            };
            user.CalendarCollections = collection;
            user.Resources = resources;
            db.Users.Add(user);
            db.SaveChanges();

            var userResult = db.GetUser("foo@gmail.com");
            Assert.NotNull(user);
            
        }
    }
}
