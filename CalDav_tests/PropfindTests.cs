using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalDAV.Core;
using CalDAV.Core.Propfind;
using DataLayer;
using Microsoft.Data.Entity;
using TreeForXml;
using Xunit;

namespace CalDav_tests
{
    public class PropfindTests
    {
        //xmlns:C=\"urn:ietf:params:xml:ns:caldav\"
        private Dictionary<string, string> Namespaces = new Dictionary<string, string> { { "D", @"xmlns:D=""DAV:""" }, { "C", @"xmlns:C=""urn:ietf:params:xml:ns:caldav\""" } };
        private Dictionary<string,string> NamespacesSimple = new Dictionary<string, string> { { "D", "DAV:"}, { "C", "urn:ietf:params:xml:ns:caldav" } }; 


        //[Fact]
        public void CreateRootWithNamespace()
        {
            ///Nachi cuando vayas a construir el primer nodo de XmlTreeStrucure
            ///le tienes que pasar el ns principal que es el que apunta el namePrefix del nodo
            /// en este caso fijate q el prefix de multistatus es D, so como este apunta a DAV:
            /// se le pasa como segundo parametro. Como tercer parametro le pasas un Dict con los 
            /// namespaces del nodo, las llaves seran los prefijos.
            XmlTreeStructure root = new XmlTreeStructure("multistatus", "DAV:", new Dictionary<string, string>
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
            propMethods.PropMethod("foo@gmail.com", "Foocollection", null, 0, new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("displayname", NamespacesSimple["D"]) }, response);
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
            propMethods.PropMethod("foo@gmail.com", "Foocollection", null, 0, new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("calendar-description", NamespacesSimple["C"]) }, response);
            IXMLTreeStructure prop;
            response.GetChildAtAnyLevel("prop", out prop);
            Assert.NotNull(prop);
            Assert.True(prop.Children.Count == 1);
            Assert.Equal(prop.Children[0].Value, "empty description");
        }

        //[Fact]
        //public void  NameTesting()
        //{
        //   // var db = MockDatabase();
        //   // var xml = db.CalendarCollections.FirstOrDefault().ResolveProperty("supported-calendar-data");
        //    //var text = xml.ToString();
        //    var text =
        //        XmlTreeStructure.Parse(
        //            $@"<C:supported-calendar-data {Namespaces["C"]}><C:comp name=""VEVENT""/><C:comp name=""VTODO""/></C:supported-calendar-data>")
        //            .ToString();
        //    Assert.Equal(text, $@"<C:supported-calendar-data {Namespaces["C"]}><C:comp name=""VEVENT""/><C:comp name=""VTODO""/></C:supported-calendar-data>");
        //}

        //[Fact]
        //public void PropCollectionGetcontenttype()
        //{
        //    var db = MockDatabase();

        //    XmlTreeStructure response = new XmlTreeStructure("multistatus", "DAV:");
        //    response.Namespaces.Add("D", "DAV:");
        //    response.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

        //    var propMethods = new CalDavPropfind(db);
        //    propMethods.PropMethod("foo@gmail.com", "Foocollection", null, 0, new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("getcontenttype", "D") }, response);
        //    IXMLTreeStructure prop;
        //    response.GetChildAtAnyLevel("prop", out prop);
        //    Assert.NotNull(prop);
        //    Assert.True(prop.Children.Count == 1);
        //    Assert.Equal(prop.Children[0].Value, "text/calendar; component=vevent");
        //}

        [Fact]
        public void PropCollectionGetetag()
        {
            var db = MockDatabase();

            XmlTreeStructure response = new XmlTreeStructure("multistatus", "DAV:");
            response.Namespaces.Add("D", "DAV:");
            response.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            var propMethods = new CalDavPropfind(db);
            propMethods.PropMethod("foo@gmail.com", "Foocollection", null, 0, new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("getetag", NamespacesSimple["D"]) }, response);
            IXMLTreeStructure prop;
            response.GetChildAtAnyLevel("prop", out prop);
            Assert.NotNull(prop);
            Assert.True(prop.Children.Count == 1);
            Assert.Equal(prop.Children[0].Value, "0");
        }

        [Fact]
        public void PropCollectionResourceType()
        {
            var db = MockDatabase();

            XmlTreeStructure response = new XmlTreeStructure("multistatus", "DAV:");
            response.Namespaces.Add("D", "DAV:");
            response.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            var propMethods = new CalDavPropfind(db);
            propMethods.PropMethod("foo@gmail.com", "Foocollection", null, 0, new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("resourcetype", NamespacesSimple["D"]) }, response);
            IXMLTreeStructure prop;
            response.GetChildAtAnyLevel("prop", out prop);
            Assert.NotNull(prop);
            Assert.True(prop.Children.Count == 1);
            Assert.Equal(prop.Children[0].Children[0].NodeName, "collection");
            Assert.Equal(prop.Children[0].Children[1].NodeName, "calendar");
        }

        #endregion

        #region Collection Propfind Test
        [Fact]
        public void ComparingCollectionFinalsXmlPropName()
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
        <D:getetag />
        <C:calendar-description />
        <D:resourcetype />
        <D:displayname />
        <D:creationdate />
      </D:prop>
    </D:propstat>
  </D:response>
</D:multistatus>";
            #endregion

            Assert.Equal(xmFinal.ToString(), trueSolution);

        }

        [Fact]
        public void ComparingCollectionFinalsXmlAllprop()
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
      </D:prop>
      <D:status>HTTP/1.1 200 OK</D:status>
    </D:propstat>
  </D:response>
</D:multistatus>";
            #endregion

            Assert.Equal(xmFinal.ToString(), trueSolution);
        }

        [Fact]
        public void ComparingCollectionFinalXmlAllPropWithInclude()
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
        <D:getetag>0</D:getetag>
      </D:prop>
      <D:status>HTTP/1.1 200 OK</D:status>
    </D:propstat>
  </D:response>
</D:multistatus>";
            #endregion

            Assert.Equal(xmFinal.ToString(), trueSolution);

        }

        [Fact]
        public void ComparingCollectionFinalXmlProp()
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
            strBuilder.AppendLine(@"<getcontentlanguage/>"); ;
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
        <D:getetag>0</D:getetag>
        <C:min-date-time>20160228T050000Z</C:min-date-time>
        <C:max-date-time>20160428T040000Z</C:max-date-time>
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
      </D:prop>
      <D:status>HTTP/1.1 200 OK</D:status>
    </D:propstat>
    <D:propstat>
      <D:prop>
        <getcontentlanguage />
        <D:getcontenttype />
      </D:prop>
      <D:status>HTTP/1.1 400 Not Found</D:status>
      <D:responsedescription>The properties doesn't  exist</D:responsedescription>
    </D:propstat>
  </D:response>
  <D:responsedescription>There has been an error</D:responsedescription>
</D:multistatus>";
            #endregion

            Assert.Equal(xmFinal.ToString(), trueSolution);
        }

        [Fact]
        public void ComparingCollectionWithResourceFinalsXmlPropName()
        {
            var db = MockDatabase();
            FileSystemManagement fs = new FileSystemManagement();
            CalDav calDav = new CalDav(fs, db);

            var prop = new Dictionary<string, string>();
            prop.Add("depth", "1");
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
        <D:getetag />
        <C:calendar-description />
        <D:displayname />
        <D:creationdate />
        <D:resourcetype />
      </D:prop>
    </D:propstat>
  </D:response>
  <D:response>
    <D:href>/api/v1/caldav/foo@gmail.com/calendars/Foocollection/test.ics</D:href>
    <D:propstat>
      <D:status>HTTP/1.1 200 OK</D:status>
      <D:prop>
        <D:resourcetype />
        <D:getcontenttype />
        <D:displayname />
        <D:creationdate />
        <D:getcontentlenght />
        <D:getetag />
        <D:getlastmodified />
        <D:getcontentlanguage />
      </D:prop>
    </D:propstat>
  </D:response>
</D:multistatus>";
            #endregion

            Assert.Equal(xmFinal.ToString(), trueSolution);

        }

        [Fact]
        public void ComparingCollectionWithResourceFinalXmlAllPropWithInclude()
        {
            var db = MockDatabase();
            FileSystemManagement fs = new FileSystemManagement();
            CalDav calDav = new CalDav(fs, db);

            var prop = new Dictionary<string, string>();
            prop.Add("depth", "1");
            prop.Add("userEmail", "foo@gmail.com");
            prop.Add("collectionName", "Foocollection");

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
            strBuilder.AppendLine(@"<D:propfind xmlns:D=""DAV:"">");
            strBuilder.AppendLine(@"<D:allprop/>");
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
      </D:prop>
      <D:status>HTTP/1.1 200 OK</D:status>
    </D:propstat>
  </D:response>
  <D:response>
    <D:href>/api/v1/caldav/foo@gmail.com/calendars/Foocollection/test.ics</D:href>
    <D:propstat>
      <D:prop>
        <D:getetag xmlns:D=""DAV:"">12345</D:getetag>
        <D:displayname xmlns:D=""DAV:"">Mocking resource</D:displayname>
        <D:creationdate xmlns:D=""DAV:"">29/03/16 01:30:44</D:creationdate>
        <D:getcontentlength xmlns:D=""DAV:"">10000</D:getcontentlength>
        <D:getcontenttype>text/icalendar</D:getcontenttype>
        <D:getlastmodified xmlns:D=""DAV:"">29/03/16 01:30:44</D:getlastmodified>
        <D:resourcetype />
      </D:prop>
      <D:status>HTTP/1.1 200 OK</D:status>
    </D:propstat>
  </D:response>
</D:multistatus>";
            #endregion

            Assert.Equal(xmFinal.ToString(), trueSolution);

        }

        [Fact]
        public void ComparingCollectionWithResourceFinalXmlProp()
        {
            var db = MockDatabase();
            FileSystemManagement fs = new FileSystemManagement();
            CalDav calDav = new CalDav(fs, db);

            var prop = new Dictionary<string, string>();
            prop.Add("depth", "1");
            prop.Add("userEmail", "foo@gmail.com");
            prop.Add("collectionName", "Foocollection");

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
            strBuilder.AppendLine(@"<D:propfind xmlns:D=""DAV:"">");
            strBuilder.AppendLine($"<prop {Namespaces["C"]}>");
            strBuilder.AppendLine(@"<C:calendar-timezone/>");
            strBuilder.AppendLine(@"<getetag/>");
            strBuilder.AppendLine(@"<getcontentlanguage/>"); ;
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
            strBuilder.AppendLine(@"<D:getcontentlenght/>");
            strBuilder.AppendLine(@"<D:getlastmodified/>");
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
        <D:getetag>0</D:getetag>
        <C:min-date-time>20160228T050000Z</C:min-date-time>
        <C:max-date-time>20160428T040000Z</C:max-date-time>
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
      </D:prop>
      <D:status>HTTP/1.1 200 OK</D:status>
    </D:propstat>
    <D:propstat>
      <D:prop>
        <getcontentlanguage />
        <D:getcontenttype />
        <D:getcontentlenght />
        <D:getlastmodified />
      </D:prop>
      <D:status>HTTP/1.1 400 Not Found</D:status>
      <D:responsedescription>The properties doesn't  exist</D:responsedescription>
    </D:propstat>
  </D:response>
  <D:response>
    <D:href>/api/v1/caldav/foo@gmail.com/calendars/Foocollection/test.ics</D:href>
    <D:propstat>
      <D:prop>
        <D:getetag xmlns:D=""DAV:"">12345</D:getetag>
        <D:getcontentlanguage xmlns:D=""DAV:"">en</D:getcontentlanguage>
        <D:getcontentlength xmlns:D=""DAV:"">10000</D:getcontentlength>
        <D:displayname xmlns:D=""DAV:"">Mocking resource</D:displayname>
        <D:resourcetype />
        <D:creationdate xmlns:D=""DAV:"">29/03/16 01:30:44</D:creationdate>
        <D:getcontenttype>text/icalendar</D:getcontenttype>
        <D:getlastmodified xmlns:D=""DAV:"">29/03/16 01:30:44</D:getlastmodified>
      </D:prop>
      <D:status>HTTP/1.1 200 OK</D:status>
    </D:propstat>
    <D:propstat>
      <D:prop>
        <D:calendar-timezone />
        <D:min-date-time />
        <D:max-date-time />
        <D:max-instances />
        <D:calendar-description />
        <D:supported-calendar-component-set />
        <D:max-resource-size />
        <D:getcontentlenght />
      </D:prop>
      <D:status>HTTP/1.1 400 Not Found</D:status>
      <D:responsedescription>The properties doesn't  exist</D:responsedescription>
    </D:propstat>
  </D:response>
  <D:responsedescription>There has been an error</D:responsedescription>
</D:multistatus>";
            #endregion

            Assert.Equal(xmFinal.ToString(), trueSolution);
        }


        #endregion



        private CalDavContext MockDatabase()
        {
            #region FIlling Database
            //FileSystemManagement fs = new FileSystemManagement();
            var optionsBuilder = new DbContextOptionsBuilder<CalDavContext>();

            // This is the magic line
            optionsBuilder.UseInMemoryDatabase();

            var db = new CalDavContext(optionsBuilder.Options);

            var user = new User
            {
                Email = "foo@gmail.com",
                LastName = "Doo",
                FirstName = "John",
                CalendarCollections = new List<CalendarCollection>()
            };
            var resources = new List<CalendarResource>
            {
                new CalendarResource
                {
                    //TODO: Adriano ver esto
                    //DtStart = DateTime.Now,
                    //DtEnd = DateTime.Now,
                    Href = "test.ics",
                    Properties = new List<ResourceProperty>
                    {
                         new ResourceProperty
                        {
                            Name = "getcontenttype",
                            Namespace = NamespacesSimple["D"],
                            Value = $"<D:getcontenttype {Namespaces["D"]}>text/icalendar</D:getcontenttype>",
                            IsVisible = true,
                            IsDestroyable = false,
                            IsMutable = true
                        },
                        new ResourceProperty
                        {
                            Name = "resourcetype",
                            Namespace = NamespacesSimple["D"],
                            Value = $"<D:resourcetype {Namespaces["D"]}/>",
                            IsVisible = true,
                            IsDestroyable = false,
                            IsMutable = true
                        },
                        new ResourceProperty
                        {
                            Name = "displayname",
                            Namespace = NamespacesSimple["D"],
                            Value = $"<D:displayname {Namespaces["D"]}>Mocking resource</D:displayname>",
                            IsVisible = true,
                            IsDestroyable = false,
                            IsMutable = true
                        },
                        new ResourceProperty
                        {
                            Name = "getetag",
                            Namespace = NamespacesSimple["D"],
                            Value = $"<D:getetag {Namespaces["D"]}>12345</D:getetag>",
                            IsVisible = true,
                            IsDestroyable = false,
                            IsMutable = true
                        },
                        new ResourceProperty
                        {
                            Name = "creationdate",
                            Namespace = NamespacesSimple["D"],
                            Value = $"<D:creationdate {Namespaces["D"]}>{"29/03/16 01:30:44"}</D:creationdate>",
                            IsVisible = true,
                            IsDestroyable = false,
                            IsMutable = true
                        },
                        new ResourceProperty
                        {
                            Name = "getcontentlanguage",
                            Namespace = NamespacesSimple["D"],
                            Value = $"<D:getcontentlanguage {Namespaces["D"]}>en</D:getcontentlanguage>",
                            IsVisible = true,
                            IsDestroyable = false,
                            IsMutable = true
                        },
                        new ResourceProperty
                        {
                            Name = "getcontentlength",
                            Namespace = NamespacesSimple["D"],
                            Value =$"<D:getcontentlength {Namespaces["D"]}>10000</D:getcontentlength>",
                            IsVisible = true,
                            IsDestroyable = false,
                            IsMutable = true
                        },
                        new ResourceProperty
                        {
                            Name = "getlastmodified",
                            Namespace = NamespacesSimple["D"],
                            Value =  $"<D:getlastmodified {Namespaces["D"]}>29/03/16 01:30:44</D:getlastmodified>",
                            IsVisible = true,
                            IsDestroyable = false,
                            IsMutable = true
                        }
                    }
                    //Displayname = $"<D:displayname {Namespaces["D"]}>Mocking resource</D:displayname>",
                    ////Recurrence = "test",
                    //User = user,
                    //Getetag = $"<D:getetag {Namespaces["D"]}>12345</D:getetag>",
                    //Creationdate =  $"<D:creationdate {Namespaces["D"]}>{"29/03/16 01:30:44"}</D:creationdate>",
                    //Getcontentlanguage = $"<D:getcontentlanguage {Namespaces["D"]}>en</D:getcontentlanguage>",
                    //Getcontentlength =  $"<D:getcontentlength {Namespaces["D"]}>10000</D:getcontentlength>",
                    //Getlastmodified = $"<D:getlastmodified {Namespaces["D"]}>29/03/16 01:30:44</D:getlastmodified>"
                }
            };
            var collection = new List<CalendarCollection>
            {
                new CalendarCollection
                {
                    //Calendardescription = "<C:calendar-description xmlns:C=\"urn:ietf:params:xml:ns:caldav\">empty description</C:calendar-description>",
                    Name = "Foocollection",
                    User = user,
                    Calendarresources = resources,/*,
                    SupportedCalendarComponentSet = new List<string>()*/
                    //ResourceType = new List<string>(),

                    //TODO: Adriano ver esto ahora es xml hecho string
                    //ResourceType = new XmlTreeStructure("resourcetype", "DAV"),
                    //Displayname = $"<D:displayname {Namespaces["D"]}>Mocking Collection</D:displayname>",
                    Url = "url",
                    //Resourcetype = $"<D:resourcetype {Namespaces["D"]}><D:collection/><C:calendar xmlns:C=\"urn:ietf:params:xml:ns:caldav\"/></D:resourcetype>",
                    //Creationdate = $"<D:creationdate {Namespaces["D"]}>{"29/03/16 01:30:44"}</D:creationdate>",
                    //Getetag = $"<D:getetag {Namespaces["D"]}>0</D:getetag>"
                    Properties = new List<CollectionProperty>
                    {
                         new CollectionProperty
                         {
                            Name= "calendar-timezone",
                            Namespace = NamespacesSimple["C"],
                            Value = $"<C:calendar-timezone {Namespaces["C"]}>LaHabana/Cuba</C:calendar-timezone>",
                            IsMutable = false,
                            IsVisible = true,
                            IsDestroyable = false
                        },
                        new CollectionProperty
                        {
                            Name= "max-resource-size",
                            Namespace = NamespacesSimple["C"],
                            Value = $"<C:max-resource-size {Namespaces["C"]}>102400</C:max-resource-size>",
                            IsMutable = false,
                            IsVisible = true,
                            IsDestroyable = false
                        },
                         new CollectionProperty
                         {
                            Name= "min-date-time",
                            Namespace = NamespacesSimple["C"],
                            Value = $"<C:min-date-time {Namespaces["C"]}>20160228T050000Z</C:min-date-time>",
                            IsMutable = false,
                            IsVisible = true,
                            IsDestroyable = false
                        },
                          new CollectionProperty
                          {
                            Name= "max-date-time",
                            Namespace = NamespacesSimple["C"],
                            Value = $"<C:max-date-time {Namespaces["C"]}>20160428T040000Z</C:max-date-time>",
                            IsMutable = false,
                            IsVisible = true,
                            IsDestroyable = false
                        },
                           new CollectionProperty
                           {
                            Name= "max-instances",
                            Namespace = NamespacesSimple["C"],
                            Value = $"<C:max-instances {Namespaces["C"]}>10</C:max-instances>",
                            IsMutable = false,
                            IsVisible = true,
                            IsDestroyable = false
                        },
                        new CollectionProperty
                        {
                            Name= "getcontentlength",
                            Namespace = NamespacesSimple["D"],
                            Value = $"<D:getcontentlength {Namespaces["D"]}>0</D:getcontentlength>",
                            IsMutable = false,
                            IsVisible = true,
                            IsDestroyable = false
                        },
                         new CollectionProperty
                        {
                            Name= "supported-calendar-component-set",
                            Namespace = NamespacesSimple["C"],
                            Value = $@"<C:supported-calendar-component-set {Namespaces["C"]}>&lt;C:comp name=""VEVENT""/&gt;&lt;C:comp name=""VTODO""/&gt;</C:supported-calendar-component-set>",
                            IsMutable = false,
                            IsVisible = true,
                            IsDestroyable = false
                        },
                          new CollectionProperty
                        {
                            Name= "supported-calendar-data",
                            Namespace = NamespacesSimple["C"],
                            Value = $@"<C:supported-calendar-data {Namespaces["C"]}><C:comp name=""VEVENT""/><C:comp name=""VTODO""/></C:supported-calendar-data>",
                            IsMutable = false,
                            IsVisible = true,
                            IsDestroyable = false
                        },
                           new CollectionProperty
                        {
                            Name= "getetag",
                            Namespace = NamespacesSimple["D"],
                            Value = $"<D:getetag {Namespaces["D"]}>0</D:getetag>",
                            IsMutable = false,
                            IsVisible = true,
                            IsDestroyable = false
                        },
                        new CollectionProperty
                        {
                            Name= "calendar-description",
                            Namespace = NamespacesSimple["C"],
                            Value = $"<C:calendar-description {Namespaces["C"]}>empty description</C:calendar-description>",
                            IsMutable = true,
                            IsVisible = true,
                            IsDestroyable = false
                        },
                        new CollectionProperty
                        {
                            Name= "resourcetype",
                            Namespace = NamespacesSimple["D"],
                            Value =  $"<D:resourcetype {Namespaces["D"]}><D:collection/><C:calendar xmlns:C=\"urn:ietf:params:xml:ns:caldav\"/></D:resourcetype>",
                            IsMutable = true,
                            IsVisible = true,
                            IsDestroyable = false
                        },
                        new CollectionProperty
                        {
                            Name= "displayname",
                            Namespace = NamespacesSimple["D"],
                            Value = $"<D:displayname {Namespaces["D"]}>Mocking Collection</D:displayname>",
                            IsMutable = true,
                            IsVisible = true,
                            IsDestroyable = false
                        },
                        new CollectionProperty
                        {
                            Name= "creationdate",
                            Namespace = NamespacesSimple["D"],
                            Value = $"<D:creationdate {Namespaces["D"]}>{"29/03/16 01:30:44"}</D:creationdate>",
                            IsMutable = true,
                            IsVisible = true,
                            IsDestroyable = false
                        }
                    }
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
