using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.CALDAV_Properties;
using CalDAV.Models;
using Xunit;

namespace CalDav_tests
{
    public class CalendarCollectionAndResourcesTest
    {
        [Fact]
        public void GetAllPropertiesName()
        {
            var user = new User()
            {
                Email = "hohn@noname.com",
                LastName = "Doe",
                FirstName = "John"

            };
            CalendarCollection colecction = new CalendarCollection()
            {
                Calendarresources = new List<CalendarResource>(),
                Resourcetype = "",
                User = user,
                Getetag = "0",
                Displayname = "collection",
                Getlastmodified = DateTime.Now.ToString(),
                Calendardescription = "empty",
                Creationdate = DateTime.Now.ToString(),
                Name = "collection",

            };

            var treeNames = colecction.GetAllPropertyNames();
            Assert.NotNull(treeNames);

            

        }

        [Fact]
        public void GetAllVisibleProperties()
        {
            
        }
    }
}
