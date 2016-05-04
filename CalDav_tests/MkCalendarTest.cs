//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Net;
//using System.Threading.Tasks;
//using CalDAV.Core;
//using DataLayer;
//using DataLayer.Models.Entities;
//using Microsoft.AspNet.Http;
//using Xunit;

//namespace CalDav_tests
//{
//    public class MkCalendarTest
//    {
//        private CalDavContext Mockdatabase()
//        {
//            var db = new CalDavContext();
//            var user = new User("Frank", "f.underwood@whitehouse.com")
//            {

//            };
//            db.Users.Add(user);
//            db.SaveChanges();
//            return db;
//        }
//        [Fact]
//        public async Task EmptyBodyCreation()
//        {
//            var fs = new FileSystemManagement();
//            var db = Mockdatabase();
//            var caldav = new CalDav(fs, db);
//            var propertiesAndHeader = new Dictionary<string, string>
//            {
//                {"userEmail", "f.underwood@whitehouse.com"},
//                {"collectionName", "DurtyPlans"},
//                {"url", "api/v1/caldav/f.underwood/calendars/durtyplans"}
//            };
//            HttpResponse response = null;

//            await caldav.MkCalendar(propertiesAndHeader, null, response);

//            Assert.Equal(response.StatusCode, (int)HttpStatusCode.Created);
//            Assert.True(string.IsNullOrEmpty(response.Body.ToString()));
//            Assert.True(fs.ExistCalendarCollection(url));
//            Assert.True(db.CollectionExist("f.underwood@whitehouse.com", "DurtyPlans"));
//        }

//        [Fact]
//        public async  Task SimpleBodyCreation()
//        {
//            var fs = new FileSystemManagement();
//            var db = Mockdatabase();
//            var caldav = new CalDav(fs, db);
//            var propertiesAndHeader = new Dictionary<string, string>
//            {
//                {"userEmail", "f.underwood@whitehouse.com"},
//                {"collectionName", "DurtyPlans"},
//                {"url", "api/v1/caldav/f.underwood/calendars/durtyplans"}
//            };
//            var body = $@"<C:mkcalendar xmlns:D=""DAV:"" xmlns:C=""urn:ietf:params:xml:ns:caldav""></C:mkcalendar>";

//            HttpResponse response = null;
//            caldav.MkCalendar(propertiesAndHeader, body, response);

//            Assert.Equal(response.StatusCode, (int)HttpStatusCode.Created);
//            Assert.True(string.IsNullOrEmpty(response.Body.ToString()));
//            Assert.True(fs.SetUserAndCollection("f.underwood@whitehouse.com", "DurtyPlans"));
//            Assert.True(db.CollectionExist("f.underwood@whitehouse.com", "DurtyPlans"));
//        }

//        [Fact]
//        public async Task ComplexBodyCreation()
//        {
//            var fs = new FileSystemManagement();
//            var db = Mockdatabase();
//            var caldav = new CalDav(fs, db);
//            var propertiesAndHeader = new Dictionary<string, string>
//            {
//                {"userEmail", "f.underwood@whitehouse.com"},
//                {"collectionName", "DurtyPlans"},
//                {"url", "api/v1/caldav/f.underwood/calendars/durtyplans"}
//            };
//            var body = $@"<C:mkcalendar xmlns:D=""DAV:"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
//  <D:set>
//    <D:prop>
//      <D:displayname>Lisa's Events</D:displayname>
//      <C:calendar-description xml:lang=""en"">Calendar restricted to events.</C:calendar-description>
//      <C:supported-calendar-component-set>
//      <C:comp name=""VEVENT""/>
//      </C:supported-calendar-component-set>
//    </D:prop>
//  </D:set>
//</C:mkcalendar>";

//            HttpResponse response = null;

//            caldav.MkCalendar(propertiesAndHeader, body, response);

//            Assert.Equal(response.StatusCode, (int)HttpStatusCode.Created);
//            Assert.True(string.IsNullOrEmpty(response.Body.ToString()));
//            Assert.True(fs.SetUserAndCollection("f.underwood@whitehouse.com", "DurtyPlans"));
//            Assert.True(db.CollectionExist("f.underwood@whitehouse.com", "DurtyPlans"));
//            var col = db.GetCollection("f.underwood@whitehouse.com", "DurtyPlans");
//            var displayname = col.Properties.SingleOrDefault(x => x.Name == "displayname");
//            Assert.True(displayname != null && displayname.Value== @"<displayname xmlns=""DAV:"">Lisa's Events</displayname>");
//        }

//        [Fact]
//        public async Task ComplexBodyFail()
//        {
//            var fs = new FileSystemManagement();
//            var db = Mockdatabase();
//            var caldav = new CalDav(fs, db);
//            var propertiesAndHeader = new Dictionary<string, string>
//            {
//                {"userEmail", "f.underwood@whitehouse.com"},
//                {"collectionName", "DurtyPlans"},
//                {"url", "api/v1/caldav/f.underwood/calendars/durtyplans"}
//            };
//            var body = $@"<C:mkcalendar xmlns:D=""DAV:"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
//  <D:set>
//    <D:prop>
//      <D:displayname>Lisa's Events</D:displayname>
//      <C:calendar-description xml:lang=""en"">Calendar restricted to events.</C:calendar-description>
//      <C:supported-calendar-component-set>
//        <C:comp name=""VEVENT""/>      
//      </C:supported-calendar-component-set>
//      <C:min-date-time>29/12/08</C:min-date-time>
//    </D:prop>
//  </D:set>
//</C:mkcalendar>";
//            HttpResponse response = null;
//             caldav.MkCalendar(propertiesAndHeader, body, response);

//            Assert.Equal(response.StatusCode, (int)HttpStatusCode.Forbidden);
//            Assert.Equal(response.Body.ToString(), @"<?xml version=""1.0"" encoding=""utf-8""?>
//<D:multistatus xmlns:D=""DAV:"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
//  <D:response>
//    <D:propstat>
//      <D:status>HTTP/1.1 424 Failed Dependency</D:status>
//      <D:prop>
//        <D:displayname />
//      </D:prop>
//    </D:propstat>
//    <D:propstat>
//      <D:status>HTTP/1.1 424 Failed Dependency</D:status>
//      <D:prop>
//        <C:calendar-description />
//      </D:prop>
//    </D:propstat>
//    <D:propstat>
//      <D:status>HTTP/1.1 424 Failed Dependency</D:status>
//      <D:prop>
//        <C:supported-calendar-component-set />
//      </D:prop>
//    </D:propstat>
//    <D:propstat>
//      <D:status>HTTP/1.1 403 Forbidden</D:status>
//      <D:prop>
//        <C:min-date-time />
//      </D:prop>
//    </D:propstat>
//  </D:response>
//</D:multistatus>");
//            Assert.True(!fs.SetUserAndCollection("f.underwood@whitehouse.com", "DurtyPlans"));
//        }
//    }
//}

