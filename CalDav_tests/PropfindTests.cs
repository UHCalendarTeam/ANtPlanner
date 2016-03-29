using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private Dictionary<string, string> Namespaces = new Dictionary<string, string>() { { "D", @"xmlns:D=""DAV:""" }, {"C", @"xmlns:C=""urn:ietf:params:xml:ns:caldav""" } };


        //[Fact]
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
        public void GetAllNames()
        {
            var db = MockDatabase();

            XmlTreeStructure response = new XmlTreeStructure("multistatus", "DAV:");
            response.Namespaces.Add("D", "DAV:");
            response.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            var propMethods = new CalDavPropfind(db);
            propMethods.PropNameMethod("foo@gmail.com", "Foocollection", null, 0, response);
            IXMLTreeStructure prop;
            response.GetChildAtAnyLevel("prop", out prop);
            Assert.NotNull(prop);
            Assert.True(prop.Children.Count > 0);
        }

        [Fact]
        public void AllVisibleNotEmpty()
        {
            var db = MockDatabase();

            XmlTreeStructure response = new XmlTreeStructure("multistatus", "DAV:");
            response.Namespaces.Add("D", "DAV:");
            response.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            var propMethods = new CalDavPropfind(db);
            propMethods.AllPropMethod("foo@gmail.com", "Foocollection", null, 0, null, response);
            IXMLTreeStructure prop;
            response.GetChildAtAnyLevel("prop", out prop);
            Assert.NotNull(prop);
            Assert.True(prop.Children.Count > 0);
        }

        [Fact]
        public void PropCollectionEmptyBody()
        {
            var db = MockDatabase();

            XmlTreeStructure response = new XmlTreeStructure("multistatus", "DAV:");
            response.Namespaces.Add("D", "DAV:");
            response.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            var propMethods = new CalDavPropfind(db);
            propMethods.PropMethod("foo@gmail.com", "Foocollection", null, 0, null, response);
            IXMLTreeStructure prop;
            response.GetChildAtAnyLevel("prop", out prop);
            Assert.Null(prop);
            //Assert.True(prop.Children.Count == 0);
        }
        #region PropFind PropMethod Prop by Prop
        [Fact]
        public void PropCollectionDisplayname()
        {
            var db = MockDatabase();

            XmlTreeStructure response = new XmlTreeStructure("multistatus", "DAV:");
            response.Namespaces.Add("D", "DAV:");
            response.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            var propMethods = new CalDavPropfind(db);
            propMethods.PropMethod("foo@gmail.com", "Foocollection", null, 0, new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("displayname", "D") }, response);
            IXMLTreeStructure prop;
            response.GetChildAtAnyLevel("prop", out prop);
            Assert.NotNull(prop);
            Assert.True(prop.Children.Count == 1);
            Assert.Equal(prop.Children[0].Value, "Mocking Collection");
        }

        [Fact]
        public void PropCollectionCalendarDescription()
        {
            var db = MockDatabase();

            XmlTreeStructure response = new XmlTreeStructure("multistatus", "DAV:");
            response.Namespaces.Add("D", "DAV:");
            response.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            var propMethods = new CalDavPropfind(db);
            propMethods.PropMethod("foo@gmail.com", "Foocollection", null, 0, new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("calendar-description", "C") }, response);
            IXMLTreeStructure prop;
            response.GetChildAtAnyLevel("prop", out prop);
            Assert.NotNull(prop);
            Assert.True(prop.Children.Count == 1);
            Assert.Equal(prop.Children[0].Value, "empty description");
        }

        [Fact]
        public void PropCollectionGetcontenttype()
        {
            var db = MockDatabase();

            XmlTreeStructure response = new XmlTreeStructure("multistatus", "DAV:");
            response.Namespaces.Add("D", "DAV:");
            response.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            var propMethods = new CalDavPropfind(db);
            propMethods.PropMethod("foo@gmail.com", "Foocollection", null, 0, new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("getcontenttype", "D") }, response);
            IXMLTreeStructure prop;
            response.GetChildAtAnyLevel("prop", out prop);
            Assert.NotNull(prop);
            Assert.True(prop.Children.Count == 1);
            Assert.Equal(prop.Children[0].Value, "text/calendar; component=vevent");
        }

        [Fact]
        public void PropCollectionGetetag()
        {
            var db = MockDatabase();

            XmlTreeStructure response = new XmlTreeStructure("multistatus", "DAV:");
            response.Namespaces.Add("D", "DAV:");
            response.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            var propMethods = new CalDavPropfind(db);
            propMethods.PropMethod("foo@gmail.com", "Foocollection", null, 0, new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("getetag", "DAV") }, response);
            IXMLTreeStructure prop;
            response.GetChildAtAnyLevel("prop", out prop);
            Assert.NotNull(prop);
            Assert.True(prop.Children.Count == 1);
            Assert.Equal(prop.Children[0].Value, "0");
        }
        [Fact]
        public void PropCollectionGetlastmodified()
        {
            var db = MockDatabase();

            XmlTreeStructure response = new XmlTreeStructure("multistatus", "DAV:");
            response.Namespaces.Add("D", "DAV:");
            response.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            var propMethods = new CalDavPropfind(db);
            propMethods.PropMethod("foo@gmail.com", "Foocollection", null, 0, new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("getlastmodified", "DAV:") }, response);
            IXMLTreeStructure prop;
            response.GetChildAtAnyLevel("prop", out prop);
            Assert.NotNull(prop);
            Assert.True(prop.Children.Count == 1);
            Assert.Equal(prop.Children[0].Value, "Mon, 12 Jan 1998 09:25:56 GMT");
        }
        [Fact]
        public void PropCollectionContentLanguange()
        {
            var db = MockDatabase();

            XmlTreeStructure response = new XmlTreeStructure("multistatus", "DAV:");
            response.Namespaces.Add("D", "DAV:");
            response.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            var propMethods = new CalDavPropfind(db);
            propMethods.PropMethod("foo@gmail.com", "Foocollection", null, 0, new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("getcontentlanguage", "DAV:") }, response);
            IXMLTreeStructure prop;
            response.GetChildAtAnyLevel("prop", out prop);
            Assert.NotNull(prop);
            Assert.True(prop.Children.Count == 1);
            Assert.Equal(prop.Children[0].Value, "en");
        }

        [Fact]
        public void PropCollectionResourceType()
        {
            var db = MockDatabase();

            XmlTreeStructure response = new XmlTreeStructure("multistatus", "DAV:");
            response.Namespaces.Add("D", "DAV:");
            response.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            var propMethods = new CalDavPropfind(db);
            propMethods.PropMethod("foo@gmail.com", "Foocollection", null, 0, new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("resourcetype", "DAV:") }, response);
            IXMLTreeStructure prop;
            response.GetChildAtAnyLevel("prop", out prop);
            Assert.NotNull(prop);
            Assert.True(prop.Children.Count == 1);
            Assert.Equal(prop.Children[0].Children[0].NodeName, "collection");
            Assert.Equal(prop.Children[0].Children[1].NodeName, "calendar");
        }

        #endregion

        [Fact]
        public void ComparingFinalsXmlPropName()
        {
            var db = MockDatabase();
            FileSystemManagement fs = new FileSystemManagement();
            CalDav calDav = new CalDav(fs, db);

            var prop = new Dictionary<string, string>();
            prop.Add("depth", "0");
            prop.Add("userEmail", "foo@gmail.com");
            prop.Add("collectionName", "Foocollection");

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8"" ?>");
            strBuilder.AppendLine(@"<propfind xmlns=""DAV:"">");
            strBuilder.AppendLine(@"<propname/>");
            strBuilder.AppendLine("</propfind>");


            var xmFinal = calDav.PropFind(prop, strBuilder.ToString());

            var strFinal = xmFinal.ToString();

            #region String solution
            var trueSolution = @"<?xml version=""1.0"" encoding=""utf-8""?>
<D:multistatus xmlns:D=""DAV:"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
  <D:response>
    <D:href>/api/v1/caldav/foo@gmail.com/calendars/Foocollection/</D:href>
    <D:propstat>
      <D:status>HTTP/1.1 200 OK</D:status>
      <D:prop>
        <C:calendar-timezone />
        <C:max-resource-size />
        <C:min-date-time />
        <C:max-date-time />
        <C:max-instances />
        <D:getcontentlength />
        <C:supported-calendar-component-set />
        <C:supported-calendar-data />
        <C:calendar-description xmlns:C=""urn:ietf:params:xml:ns:caldav"" />
        <D:displayname />
        <D:resourcetype />
        <C:calendar-timezone />
        <C:min-date-time />
        <C:max-date-time />
        <C:max-intances />
      </D:prop>
    </D:propstat>
  </D:response>
</D:multistatus>";
            #endregion

            Assert.Equal(xmFinal.ToString(), trueSolution);

        }

        [Fact]
        public void ComparingFinalsXmlAllprop()
        {
            var db = MockDatabase();
            FileSystemManagement fs = new FileSystemManagement();
            CalDav calDav = new CalDav(fs, db);

            var prop = new Dictionary<string, string>();
            prop.Add("depth", "0");
            prop.Add("userEmail", "foo@gmail.com");
            prop.Add("collectionName", "Foocollection");

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
            strBuilder.AppendLine(@"<propfind xmlns=""DAV:"">");
            strBuilder.AppendLine(@"<allprop/>");
            strBuilder.AppendLine("</propfind>");


            var xmFinal = calDav.PropFind(prop, strBuilder.ToString());

            var strFinal = xmFinal.ToString();

            #region String solution
            var trueSolution = @"<?xml version=""1.0"" encoding=""utf-8""?>
<D:multistatus xmlns:D=""DAV:"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
  <D:response>
    <D:href>/api/v1/caldav/foo@gmail.com/calendars/Foocollection/</D:href>
    <D:propstat>
      <D:prop>
        <D:displayname xmlns:D=""DAV:"">Mocking Collection</D:displayname>
        <D:resourcetype xmlns:D=""DAV:"">
          <D:collection />
          <C:calendar xmlns:C=""urn:ietf:params:xml:ns:caldav"" />
        </D:resourcetype>
        <D:creationdate xmlns:D=""DAV:"">29/03/16 01:30:44</D:creationdate>
        <C:calendar-description xmlns:C=""urn:ietf:params:xml:ns:caldav"">empty description</C:calendar-description>
        <D:getcontenttype xmlns:D=""DAV:"">text/calendar; component=vevent</D:getcontenttype>
      </D:prop>
      <D:status>HTTP/1.1 200 OK</D:status>
    </D:propstat>
  </D:response>
</D:multistatus>";
            #endregion

            Assert.Equal(xmFinal.ToString(), trueSolution);
        }

        [Fact]
        public void ComparingFinalXmlAllPropWithInclude()
        {
            var db = MockDatabase();
            FileSystemManagement fs = new FileSystemManagement();
            CalDav calDav = new CalDav(fs, db);

            var prop = new Dictionary<string, string>();
            prop.Add("depth", "0");
            prop.Add("userEmail", "foo@gmail.com");
            prop.Add("collectionName", "Foocollection");

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
            strBuilder.AppendLine(@"<D:propfind xmlns:D=""DAV:"">");
            strBuilder.AppendLine(@"<D:allprop/>");
            strBuilder.AppendLine(@"<D:include>");
            strBuilder.AppendLine(@"<D:getetag/>");
            strBuilder.AppendLine(@"<D:getcontentlanguage/>");
            strBuilder.AppendLine(@"</D:include>");
            strBuilder.AppendLine("</D:propfind>");


            var xmFinal = calDav.PropFind(prop, strBuilder.ToString());

            var strFinal = xmFinal.ToString();

            #region String solution
            var trueSolution = @"<?xml version=""1.0"" encoding=""utf-8""?>
<D:multistatus xmlns:D=""DAV:"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
  <D:response>
    <D:href>/api/v1/caldav/foo@gmail.com/calendars/Foocollection/</D:href>
    <D:propstat>
      <D:prop>
        <D:displayname xmlns:D=""DAV:"">Mocking Collection</D:displayname>
        <D:resourcetype xmlns:D=""DAV:"">
          <D:collection />
          <C:calendar xmlns:C=""urn:ietf:params:xml:ns:caldav"" />
        </D:resourcetype>
        <D:creationdate xmlns:D=""DAV:"">29/03/16 01:30:44</D:creationdate>
        <C:calendar-description xmlns:C=""urn:ietf:params:xml:ns:caldav"">empty description</C:calendar-description>
        <D:getcontenttype xmlns:D=""DAV:"">text/calendar; component=vevent</D:getcontenttype>
        <D:getetag xmlns:D=""DAV:"">0</D:getetag>
        <D:getcontentlanguage xmlns:D=""DAV:"">en</D:getcontentlanguage>
      </D:prop>
      <D:status>HTTP/1.1 200 OK</D:status>
    </D:propstat>
  </D:response>
</D:multistatus>";
            #endregion

            Assert.Equal(xmFinal.ToString(), trueSolution);

        }

        [Fact]
        public void ComparingFinalXmlProp()
        {
            var db = MockDatabase();
            FileSystemManagement fs = new FileSystemManagement();
            CalDav calDav = new CalDav(fs, db);

            var prop = new Dictionary<string, string>();
            prop.Add("depth", "0");
            prop.Add("userEmail", "foo@gmail.com");
            prop.Add("collectionName", "Foocollection");

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
            strBuilder.AppendLine(@"<D:propfind xmlns:D=""DAV:"">");
            strBuilder.AppendLine($"<prop {Namespaces["C"]}>");
            strBuilder.AppendLine(@"<C:calendar-timezone/>");
            strBuilder.AppendLine(@"<getetag/>");
            strBuilder.AppendLine(@"<getcontentlanguage/>");;
            strBuilder.AppendLine(@"<C:min-date-time/>");
            strBuilder.AppendLine(@"<C:max-date-time/>");
            strBuilder.AppendLine(@"<C:max-instances/>");
            strBuilder.AppendLine(@"<getcontentlength/>");
            strBuilder.AppendLine(@"<C:calendar-description/>");
            strBuilder.AppendLine(@"<displayname/>");
            strBuilder.AppendLine(@"<resourcetype/>");
            strBuilder.AppendLine(@"<C:supported-calendar-component-set/>");
            strBuilder.AppendLine(@"<C:max-resource-size/>");
            strBuilder.AppendLine(@"<D:creationdate/>");
            strBuilder.AppendLine(@"<D:getcontenttype/>");
            strBuilder.AppendLine(@"</prop>");
            strBuilder.AppendLine("</D:propfind>");

            var xmFinal = calDav.PropFind(prop, strBuilder.ToString());

            var strFinal = xmFinal.ToString();

            #region String solution
            var trueSolution = @"<?xml version=""1.0"" encoding=""utf-8""?>
<D:multistatus xmlns:D=""DAV:"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
  <D:response>
    <D:href>/api/v1/caldav/foo@gmail.com/calendars/Foocollection/</D:href>
    <D:propstat>
      <D:prop>
        <C:calendar-timezone>LaHabana/Cuba</C:calendar-timezone>
        <D:getetag xmlns:D=""DAV:"">0</D:getetag>
        <D:getcontentlanguage xmlns:D=""DAV:"">en</D:getcontentlanguage>
        <C:min-date-time>20160229T050000Z</C:min-date-time>
        <C:max-date-time>20160429T040000Z</C:max-date-time>
        <C:max-instances>10</C:max-instances>
        <D:getcontentlength>0</D:getcontentlength>
        <C:calendar-description xmlns:C=""urn:ietf:params:xml:ns:caldav"">empty description</C:calendar-description>
        <D:displayname xmlns:D=""DAV:"">Mocking Collection</D:displayname>
        <D:resourcetype xmlns:D=""DAV:"">
          <D:collection />
          <C:calendar xmlns:C=""urn:ietf:params:xml:ns:caldav"" />
        </D:resourcetype>
        <C:supported-calendar-component-set>&lt;C:comp name=""VEVENT""/&gt;&lt;C:comp name=""VTODO""/&gt;</C:supported-calendar-component-set>
        <C:max-resource-size>102400</C:max-resource-size>
        <D:creationdate xmlns:D=""DAV:"">29/03/16 01:30:44</D:creationdate>
        <D:getcontenttype xmlns:D=""DAV:"">text/calendar; component=vevent</D:getcontenttype>
      </D:prop>
      <D:status>HTTP/1.1 200 OK</D:status>
    </D:propstat>
  </D:response>
</D:multistatus>";
            #endregion

            Assert.Equal(xmFinal.ToString(), trueSolution);
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
                    Creationdate = "29/03/16 01:30:44",


                }
            };
            var collection = new List<CalendarCollection>()
            {
                new CalendarCollection()
                {
                    Calendardescription = "<C:calendar-description xmlns:C=\"urn:ietf:params:xml:ns:caldav\">empty description</C:calendar-description>",
                    Name = "Foocollection",
                    User = user,
                    Calendarresources = resources/*,
                    SupportedCalendarComponentSet = new List<string>()*/,
                    //ResourceType = new List<string>(),

                    //TODO: Adriano ver esto ahora es xml hecho string
                    //ResourceType = new XmlTreeStructure("resourcetype", "DAV"),
                    Displayname = $"<D:displayname {Namespaces["D"]}>Mocking Collection</D:displayname>",
                    Getcontenttype = $"<D:getcontenttype {Namespaces["D"]}>text/calendar; component=vevent</D:getcontenttype>",
                    Url = "url",
                    Resourcetype = $"<D:resourcetype {Namespaces["D"]}><D:collection/><C:calendar xmlns:C=\"urn:ietf:params:xml:ns:caldav\"/></D:resourcetype>",
                    Creationdate = $"<D:creationdate {Namespaces["D"]}>{"29/03/16 01:30:44"}</D:creationdate>",
                    Getetag = $"<D:getetag {Namespaces["D"]}>0</D:getetag>",
                    Getlastmodified = $"<D:getlastmodified {Namespaces["D"]}>Mon, 12 Jan 1998 09:25:56 GMT</D:getlastmodified>",
                    Getcontentlanguage = $"<D:getcontentlanguage {Namespaces["D"]}>en</D:getcontentlanguage>"


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
