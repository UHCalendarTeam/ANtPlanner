using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TreeForXml;
using CalDAV.Core;
using CalDAV.Core.Propfind;
using Xunit;
using Microsoft.Data.Entity;
using CalDAV.Models;

namespace CalDav_tests
{
    public class PropfindTests
    {
        [Fact]
        public void CreateRootWithNamespace()
        {
            ///Nachi cuando vayas a construir el primer nodo de XmlTreeStrucure
            ///le tienes que pasar el ns principal que es el que apunta el namePrefix del nodo
            /// en este caso fijate q el prefix de multistatus es D, so como este apunta a DAV:
            /// se le pasa como segundo parametro. Como tercer parametro le pasas un Dict con los 
            /// namespaces del nodo, las llaves seran los prefijos.
            XmlTreeStructure root = new XmlTreeStructure("multistatus", "DAV:", new Dictionary<string, string>()
            {
                {"D", "DAV:" },
                {"C", "urn:ietf:params:xml:ns:caldav" }
            });

        }

        [Fact]
        public void PropFindGetAllNames()
        {
            var db = MockDatabase();

            XmlTreeStructure response = new XmlTreeStructure("multistatus", "DAV:");
            response.Namespaces.Add("D", "DAV:");
            response.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            var propMethods = new CalDavPropfind(db);
            propMethods.PropNameMethod("foo@gmail.com", "Foocollection", null, 0, response);
            var prop = response.GetChildAtAnyLevel("prop");
            Assert.NotNull(prop);
            Assert.True(prop.Children.Count > 0);
        }
        [Fact]
        public void PropFindAllVisible()
        {
            var db = MockDatabase();

            XmlTreeStructure response = new XmlTreeStructure("multistatus", "DAV:");
            response.Namespaces.Add("D", "DAV:");
            response.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            var propMethods = new CalDavPropfind(db);
            propMethods.AllPropMethod("foo@gmail.com", "Foocollection", null, 0, null, response);
            var prop = response.GetChildAtAnyLevel("prop");
            Assert.NotNull(prop);
            Assert.True(prop.Children.Count > 0);
        }

        private CalDavContext MockDatabase()
        {
            #region FIlling Database

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
                    Calendardescription = " <C:calendar-description xmlns:C=\"urn:ietf:params:xml: ns: caldav\">empty description</C:calendar-description>",
                    Name = "Foocollection",
                    User = user,
                    Calendarresources = resources/*,
                    SupportedCalendarComponentSet = new List<string>()*/,
                    //ResourceType = new List<string>(),

                    //TODO: Adriano ver esto ahora es xml hecho string
                    //ResourceType = new XmlTreeStructure("resourcetype", "DAV"),
                    Displayname = "<D:displayname>Mocking Collection</D:displayname>",
                    GetContenttype = " <D:getcontenttype>text/calendar; component=vevent</D:getcontenttype>",
                    Url = "url",
                    Resourcetype = "<D:resourcetype><D:collection />< C:calendar xmlns:C = \"urn:ietf:params:xml:ns:caldav\" /></ D:resourcetype > "

                }
            };
            user.CalendarCollections = collection;
            user.Resources = resources;
            db.Users.Add(user);
            db.SaveChanges();
            return db;

            #endregion
        }


    }
}
