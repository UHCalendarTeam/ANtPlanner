using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CalDAV.Core;
using DataLayer;
using DataLayer.Entities;
using Xunit;

namespace CalDav_tests
{
    public class MkCalendarTest
    {
        private CalDavContext Mockdatabase()
        {
            var db = new CalDavContext();
            var user = new User("Frank", "f.underwood@whitehouse.com")
            {
                LastName = "Underwood"
            };
            db.Users.Add(user);
            db.SaveChanges();
            return db;
        }
        [Fact]
        public void EmptyBodyCreation()
        {
            var fs = new FileSystemManagement();
            var db = Mockdatabase();
            var caldav = new CalDav(fs, db);
            var propertiesAndHeader = new Dictionary<string, string>
            {
                {"userEmail", "f.underwood@whitehouse.com"},
                {"collectionName", "DurtyPlans"},
                {"url", "api/v1/caldav/f.underwood/calendars/durtyplans"}
            };

            var mkres = caldav.MkCalendar(propertiesAndHeader, null);

            Assert.Equal(mkres.Key,HttpStatusCode.Created);
            Assert.True(string.IsNullOrEmpty(mkres.Value));
            Assert.True(fs.SetUserAndCollection("f.underwood@whitehouse.com","DurtyPlans"));
            Assert.True(db.CollectionExist("f.underwood@whitehouse.com", "DurtyPlans"));
        }

        [Fact]
        public void SimpleBodyCreation()
        {
            var fs = new FileSystemManagement();
            var db = Mockdatabase();
            var caldav = new CalDav(fs, db);
            var propertiesAndHeader = new Dictionary<string, string>
            {
                {"userEmail", "f.underwood@whitehouse.com"},
                {"collectionName", "DurtyPlans"},
                {"url", "api/v1/caldav/f.underwood/calendars/durtyplans"}
            };
            var body = $@"<C:mkcalendar xmlns:D=""DAV:"" xmlns:C=""urn:ietf:params:xml:ns:caldav""></C:mkcalendar>";

            var mkres = caldav.MkCalendar(propertiesAndHeader, body);

            Assert.Equal(mkres.Key, HttpStatusCode.Created);
            Assert.True(string.IsNullOrEmpty(mkres.Value));
            Assert.True(fs.SetUserAndCollection("f.underwood@whitehouse.com", "DurtyPlans"));
            Assert.True(db.CollectionExist("f.underwood@whitehouse.com", "DurtyPlans"));
        }

        [Fact]
        public void ComplexBodyCreation()
        {
            var fs = new FileSystemManagement();
            var db = Mockdatabase();
            var caldav = new CalDav(fs, db);
            var propertiesAndHeader = new Dictionary<string, string>
            {
                {"userEmail", "f.underwood@whitehouse.com"},
                {"collectionName", "DurtyPlans"},
                {"url", "api/v1/caldav/f.underwood/calendars/durtyplans"}
            };
            var body = $@"<C:mkcalendar xmlns:D=""DAV:"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
  <D:set>
    <D:prop>
      <D:displayname>Lisa's Events</D:displayname>
      <C:calendar-description xml:lang=""en"">Calendar restricted to events.</C:calendar-description>
      <C:supported-calendar-component-set>
      <C:comp name=""VEVENT""/>
      </C:supported-calendar-component-set>
    </D:prop>
  </D:set>
</C:mkcalendar>";

            var mkres = caldav.MkCalendar(propertiesAndHeader, body);

            Assert.Equal(mkres.Key, HttpStatusCode.Created);
            Assert.True(string.IsNullOrEmpty(mkres.Value));
            Assert.True(fs.SetUserAndCollection("f.underwood@whitehouse.com", "DurtyPlans"));
            Assert.True(db.CollectionExist("f.underwood@whitehouse.com", "DurtyPlans"));
            var col = db.GetCollection("f.underwood@whitehouse.com", "DurtyPlans");
            var displayname = col.Properties.SingleOrDefault(x => x.Name == "displayname");
            Assert.True(displayname != null && displayname.Value== @"<displayname xmlns=""DAV:"">Lisa's Events</displayname>");
        }

        [Fact]
        public void ComplexBodyFail()
        {
            var fs = new FileSystemManagement();
            var db = Mockdatabase();
            var caldav = new CalDav(fs, db);
            var propertiesAndHeader = new Dictionary<string, string>
            {
                {"userEmail", "f.underwood@whitehouse.com"},
                {"collectionName", "DurtyPlans"},
                {"url", "api/v1/caldav/f.underwood/calendars/durtyplans"}
            };
            var body = $@"<C:mkcalendar xmlns:D=""DAV:"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
  <D:set>
    <D:prop>
      <D:displayname>Lisa's Events</D:displayname>
      <C:calendar-description xml:lang=""en"">Calendar restricted to events.</C:calendar-description>
      <C:supported-calendar-component-set>
        <C:comp name=""VEVENT""/>      
      </C:supported-calendar-component-set>
      <C:min-date-time>29/12/08</C:min-date-time>
    </D:prop>
  </D:set>
</C:mkcalendar>";

            var mkres = caldav.MkCalendar(propertiesAndHeader, body);

            Assert.Equal(mkres.Key, HttpStatusCode.Forbidden);
            Assert.Equal(mkres.Value, @"<?xml version=""1.0"" encoding=""utf-8""?>
<D:multistatus xmlns:D=""DAV:"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
  <D:response>
    <D:propstat>
      <D:status>HTTP/1.1 424 Failed Dependency</D:status>
      <D:prop>
        <D:displayname />
      </D:prop>
    </D:propstat>
    <D:propstat>
      <D:status>HTTP/1.1 424 Failed Dependency</D:status>
      <D:prop>
        <C:calendar-description />
      </D:prop>
    </D:propstat>
    <D:propstat>
      <D:status>HTTP/1.1 424 Failed Dependency</D:status>
      <D:prop>
        <C:supported-calendar-component-set />
      </D:prop>
    </D:propstat>
    <D:propstat>
      <D:status>HTTP/1.1 403 Forbidden</D:status>
      <D:prop>
        <C:min-date-time />
      </D:prop>
    </D:propstat>
  </D:response>
</D:multistatus>");
            Assert.True(!fs.SetUserAndCollection("f.underwood@whitehouse.com", "DurtyPlans"));
        }
    }
}
