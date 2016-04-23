//using System.Collections.Generic;
//using System.Linq;
//using CalDAV.Core;
//using DataLayer;
//using DataLayer.Models.Entities;
//using Microsoft.AspNet.Http;
//using TreeForXml;
//using Xunit;

//namespace CalDav_tests
//{
//    public class ProppatchTest
//    {
//        private readonly Dictionary<string, string> Namespaces = new Dictionary<string, string>
//        {
//            {"D", @"xmlns:D=""DAV:"""},
//            {"C", @"xmlns:C=""urn:ietf:params:xml:ns:caldav"""}
//        };

//        private readonly Dictionary<string, string> NamespacesSimple = new Dictionary<string, string>
//        {
//            {"D", "DAV:"},
//            {"C", "urn:ietf:params:xml:ns:caldav"}
//        };

//        private CalDavContext MockDatabase()
//        {
//            #region FIlling Database

//            //FileSystemManagement fs = new FileSystemManagement();
//            //var optionsBuilder = new DbContextOptionsBuilder<CalDavContext>();

//            // This is the magic line
//            // optionsBuilder.UseInMemoryDatabase();

//            var db = new CalDavContext(); //optionsBuilder.Options);
//            //db.Database.EnsureDeleted();
//            //db.Database.EnsureCreated();

//            var user = new User
//            {
//                Email = "foo@gmail.com",
                
//            };
//            var resources = new List<CalendarResource>
//            {
//                new CalendarResource
//                {
//                    Name = "test.ics",
//                    Href = "....",
//                    Properties = new List<Property>
//                    {
//                        new Property
//                        {
//                            Name = "getcontenttype",
//                            Namespace = NamespacesSimple["D"],
//                            Value = $"<D:getcontenttype {Namespaces["D"]}>text/icalendar</D:getcontenttype>",
//                            IsVisible = true,
//                            IsDestroyable = false,
//                            IsMutable = true
//                        },
//                        new Property
//                        {
//                            Name = "resourcetype",
//                            Namespace = NamespacesSimple["D"],
//                            Value = $"<D:resourcetype {Namespaces["D"]}/>",
//                            IsVisible = true,
//                            IsDestroyable = false,
//                            IsMutable = true
//                        },
//                        new Property
//                        {
//                            Name = "displayname",
//                            Namespace = NamespacesSimple["D"],
//                            Value = $"<D:displayname {Namespaces["D"]}>Mocking resource</D:displayname>",
//                            IsVisible = true,
//                            IsDestroyable = false,
//                            IsMutable = true
//                        },
//                        new Property
//                        {
//                            Name = "getetag",
//                            Namespace = NamespacesSimple["D"],
//                            Value = $"<D:getetag {Namespaces["D"]}>12345</D:getetag>",
//                            IsVisible = true,
//                            IsDestroyable = false,
//                            IsMutable = true
//                        },
//                        new Property
//                        {
//                            Name = "creationdate",
//                            Namespace = NamespacesSimple["D"],
//                            Value = $"<D:creationdate {Namespaces["D"]}>{"29/03/16 01:30:44"}</D:creationdate>",
//                            IsVisible = true,
//                            IsDestroyable = false,
//                            IsMutable = true
//                        },
//                        new Property
//                        {
//                            Name = "getcontentlanguage",
//                            Namespace = NamespacesSimple["D"],
//                            Value = $"<D:getcontentlanguage {Namespaces["D"]}>en</D:getcontentlanguage>",
//                            IsVisible = true,
//                            IsDestroyable = false,
//                            IsMutable = true
//                        },
//                        new Property
//                        {
//                            Name = "getcontentlength",
//                            Namespace = NamespacesSimple["D"],
//                            Value = $"<D:getcontentlength {Namespaces["D"]}>10000</D:getcontentlength>",
//                            IsVisible = true,
//                            IsDestroyable = false,
//                            IsMutable = true
//                        },
//                        new Property
//                        {
//                            Name = "getlastmodified",
//                            Namespace = NamespacesSimple["D"],
//                            Value = $"<D:getlastmodified {Namespaces["D"]}>29/03/16 01:30:44</D:getlastmodified>",
//                            IsVisible = true,
//                            IsDestroyable = false,
//                            IsMutable = true
//                        }
//                    }
//                }
//            };
//            var collection = new List<CalendarCollection>
//            {
//                new CalendarCollection
//                {
//                    Name = "Foocollection",
//                    //User = user,
//                    CalendarResources = resources,
//                    Url = "url",
//                    Properties = new List<Property>
//                    {
//                        new Property
//                        {
//                            Name = "calendar-timezone",
//                            Namespace = NamespacesSimple["C"],
//                            Value = $"<C:calendar-timezone {Namespaces["C"]}>LaHabana/Cuba</C:calendar-timezone>",
//                            IsMutable = false,
//                            IsVisible = true,
//                            IsDestroyable = false
//                        },
//                        new Property
//                        {
//                            Name = "max-resource-size",
//                            Namespace = NamespacesSimple["C"],
//                            Value = $"<C:max-resource-size {Namespaces["C"]}>102400</C:max-resource-size>",
//                            IsMutable = false,
//                            IsVisible = true,
//                            IsDestroyable = false
//                        },
//                        new Property
//                        {
//                            Name = "min-date-time",
//                            Namespace = NamespacesSimple["C"],
//                            Value = $"<C:min-date-time {Namespaces["C"]}>20160228T050000Z</C:min-date-time>",
//                            IsMutable = false,
//                            IsVisible = true,
//                            IsDestroyable = false
//                        },
//                        new Property
//                        {
//                            Name = "max-date-time",
//                            Namespace = NamespacesSimple["C"],
//                            Value = $"<C:max-date-time {Namespaces["C"]}>20160428T040000Z</C:max-date-time>",
//                            IsMutable = false,
//                            IsVisible = true,
//                            IsDestroyable = false
//                        },
//                        new Property
//                        {
//                            Name = "max-instances",
//                            Namespace = NamespacesSimple["C"],
//                            Value = $"<C:max-instances {Namespaces["C"]}>10</C:max-instances>",
//                            IsMutable = false,
//                            IsVisible = true,
//                            IsDestroyable = false
//                        },
//                        new Property
//                        {
//                            Name = "getcontentlength",
//                            Namespace = NamespacesSimple["D"],
//                            Value = $"<D:getcontentlength {Namespaces["D"]}>0</D:getcontentlength>",
//                            IsMutable = false,
//                            IsVisible = true,
//                            IsDestroyable = false
//                        },
//                        new Property
//                        {
//                            Name = "supported-calendar-component-set",
//                            Namespace = NamespacesSimple["C"],
//                            Value =
//                                $@"<C:supported-calendar-component-set {Namespaces["C"]}>&lt;C:comp name=""VEVENT""/&gt;&lt;C:comp name=""VTODO""/&gt;</C:supported-calendar-component-set>",
//                            IsMutable = false,
//                            IsVisible = true,
//                            IsDestroyable = false
//                        },
//                        new Property
//                        {
//                            Name = "supported-calendar-data",
//                            Namespace = NamespacesSimple["C"],
//                            Value =
//                                $@"<C:supported-calendar-data {Namespaces["C"]}><C:comp name=""VEVENT""/><C:comp name=""VTODO""/></C:supported-calendar-data>",
//                            IsMutable = false,
//                            IsVisible = true,
//                            IsDestroyable = false
//                        },
//                        new Property
//                        {
//                            Name = "getetag",
//                            Namespace = NamespacesSimple["D"],
//                            Value = $"<D:getetag {Namespaces["D"]}>0</D:getetag>",
//                            IsMutable = false,
//                            IsVisible = true,
//                            IsDestroyable = false
//                        },
//                        new Property
//                        {
//                            Name = "calendar-description",
//                            Namespace = NamespacesSimple["C"],
//                            Value =
//                                $"<C:calendar-description {Namespaces["C"]}>empty description</C:calendar-description>",
//                            IsMutable = true,
//                            IsVisible = true,
//                            IsDestroyable = false
//                        },
//                        new Property
//                        {
//                            Name = "resourcetype",
//                            Namespace = NamespacesSimple["D"],
//                            Value =
//                                $"<D:resourcetype {Namespaces["D"]}><D:collection/><C:calendar xmlns:C=\"urn:ietf:params:xml:ns:caldav\"/></D:resourcetype>",
//                            IsMutable = true,
//                            IsVisible = true,
//                            IsDestroyable = false
//                        },
//                        new Property
//                        {
//                            Name = "displayname",
//                            Namespace = NamespacesSimple["D"],
//                            Value = $"<D:displayname {Namespaces["D"]}>Mocking Collection</D:displayname>",
//                            IsMutable = true,
//                            IsVisible = true,
//                            IsDestroyable = false
//                        },
//                        new Property
//                        {
//                            Name = "creationdate",
//                            Namespace = NamespacesSimple["D"],
//                            Value = $"<D:creationdate {Namespaces["D"]}>{"29/03/16 01:30:44"}</D:creationdate>",
//                            IsMutable = true,
//                            IsVisible = true,
//                            IsDestroyable = false
//                        }
//                    }
//                }
//            };
//            user.CalendarCollections = collection;
//            //user.Resources = resources;
//            db.Users.Add(user);
//            db.SaveChanges();
//            return db;

//            #endregion
//        }

//        [Fact]
//        public void ModifyPropertySuccesful()
//        {
//            var db = MockDatabase();
//            var fs = new FileSystemManagement();

//            var caldav = new CalDav(fs, db);
//            var propertiesAndHeaders = new Dictionary<string, string>
//            {
//                {"userEmail", "foo@gmail.com"},
//                {"collectionName", "Foocollection"}
//            };
//            var body =
//                $@"<propertyupdate {Namespaces["D"]} {Namespaces["C"]}>
//  <set>
//    <prop>
//      <C:calendar-description>void description</C:calendar-description>
//    </prop>
//  </set>
//</propertyupdate>";

//            var request = XmlTreeStructure.Parse(body);
//            HttpResponse response = null;
//            caldav.PropPatch(propertiesAndHeaders, request.ToString(), response);

//            Assert.Equal(response.Body.ToString(),
//                @"<?xml version=""1.0"" encoding=""utf-8""?>
//<D:multistatus xmlns:D=""DAV:"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
//  <D:response>
//    <D:href>/api/v1/caldav/foo@gmail.com/calendars/Foocollection/</D:href>
//    <D:propstat>
//      <D:status>HTTP/1.1 200 OK</D:status>
//      <D:prop>
//        <C:calendar-description />
//      </D:prop>
//    </D:propstat>
//  </D:response>
//</D:multistatus>");
//            using (db = new CalDavContext())
//            {
//                var testCollection = db.GetCollection("foo@gmail.com", "Foocollection");
//                var testPropperty = testCollection.Properties
//                    .SingleOrDefault(x => x.Name == "calendar-description" && x.Namespace == NamespacesSimple["C"]);

//                Assert.Equal(testPropperty.Value,
//                    $@"<calendar-description xmlns=""{NamespacesSimple["C"]}"">void description</calendar-description>");

//                var varCorrectParsing = XmlTreeStructure.Parse(testPropperty.Value);
//                Assert.NotNull(varCorrectParsing);
//            }
//        }

//        [Fact]
//        public void CreateSuccesful()
//        {
//            var db = MockDatabase();
//            var fs = new FileSystemManagement();

//            var caldav = new CalDav(fs, db);
//            var propertiesAndHeaders = new Dictionary<string, string>
//            {
//                {"userEmail", "foo@gmail.com"},
//                {"collectionName", "Foocollection"}
//            };
//            var body =
//                $@"<propertyupdate {Namespaces["D"]} {Namespaces["C"]}>
//  <set>
//    <prop>
//      <C:calendar-test>test 2</C:calendar-test>
//    </prop>
//  </set>
//</propertyupdate>";

//            var request = XmlTreeStructure.Parse(body);
//            HttpResponse response = null;
//            caldav.PropPatch(propertiesAndHeaders, request.ToString(), response);

//            Assert.Equal(response.Body.ToString(),
//                @"<?xml version=""1.0"" encoding=""utf-8""?>
//<D:multistatus xmlns:D=""DAV:"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
//  <D:response>
//    <D:href>/api/v1/caldav/foo@gmail.com/calendars/Foocollection/</D:href>
//    <D:propstat>
//      <D:status>HTTP/1.1 200 OK</D:status>
//      <D:prop>
//        <C:calendar-test />
//      </D:prop>
//    </D:propstat>
//  </D:response>
//</D:multistatus>");

//            using (db = new CalDavContext())
//            {
//                var testCollection = db.GetCollection("foo@gmail.com", "Foocollection");
//                var testPropperty = testCollection.Properties
//                    .SingleOrDefault(x => x.Name == "calendar-test" && x.Namespace == NamespacesSimple["C"]);

//                Assert.Equal(testPropperty.Value,
//                    $@"<calendar-test xmlns=""{NamespacesSimple["C"]}"">test 2</calendar-test>");

//                var varCorrectParsing = XmlTreeStructure.Parse(testPropperty.Value);
//                Assert.NotNull(varCorrectParsing);
//            }
//        }

//        [Fact]
//        public void CreateAndThenRemoveSuccesful()
//        {
//            var db = MockDatabase();
//            var fs = new FileSystemManagement();

//            var caldav = new CalDav(fs, db);
//            var propertiesAndHeaders = new Dictionary<string, string>
//            {
//                {"userEmail", "foo@gmail.com"},
//                {"collectionName", "Foocollection"}
//            };
//            var body =
//                $@"<propertyupdate {Namespaces["D"]} {Namespaces["C"]}>
//  <set>
//    <prop>
//      <C:calendar-test>test 2</C:calendar-test>
//    </prop>
//  </set>
//  <remove>
//    <prop>
//      <C:calendar-test/>
//    </prop>
//  </remove>
//</propertyupdate>";

//            var request = XmlTreeStructure.Parse(body);
//            HttpResponse response = null;

//            caldav.PropPatch(propertiesAndHeaders, request.ToString(), response);

//            Assert.Equal(response.Body.ToString(),
//                @"<?xml version=""1.0"" encoding=""utf-8""?>
//<D:multistatus xmlns:D=""DAV:"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
//  <D:response>
//    <D:href>/api/v1/caldav/foo@gmail.com/calendars/Foocollection/</D:href>
//    <D:propstat>
//      <D:status>HTTP/1.1 200 OK</D:status>
//      <D:prop>
//        <C:calendar-test />
//      </D:prop>
//    </D:propstat>
//    <D:propstat>
//      <D:status>HTTP/1.1 200 OK</D:status>
//      <D:prop>
//        <C:calendar-test />
//      </D:prop>
//    </D:propstat>
//  </D:response>
//</D:multistatus>");

//            using (db = new CalDavContext())
//            {
//                var testCollection = db.GetCollection("foo@gmail.com", "Foocollection");
//                var testPropperty = testCollection.Properties
//                    .SingleOrDefault(x => x.Name == "calendar-test" && x.Namespace == NamespacesSimple["C"]);

//                Assert.Null(testPropperty);
//            }
//        }

//        [Fact]
//        public void SeveralSetAndRemoveSuccesful()
//        {
//        }

//        [Fact]
//        public void ErrorCoctel()
//        {
//            var db = MockDatabase();
//            var fs = new FileSystemManagement();

//            var caldav = new CalDav(fs, db);
//            var propertiesAndHeaders = new Dictionary<string, string>
//            {
//                {"userEmail", "foo@gmail.com"},
//                {"collectionName", "Foocollection"}
//            };
//            var body =
//                $@"<propertyupdate {Namespaces["D"]} {Namespaces["C"]}>
//  <set>
//    <prop>
//      <C:calendar-test>test 2</C:calendar-test>
//    </prop>
//  </set>
//  <remove>
//    <prop>
//      <C:calendar-description/>
//    </prop>
//  </remove>
//  <set>
//    <prop>
//      <C:calendar-test>test failed</C:calendar-test>
//    </prop>
//  </set>
//</propertyupdate>";

//            var request = XmlTreeStructure.Parse(body);
//            HttpResponse response = null;

//            caldav.PropPatch(propertiesAndHeaders, request.ToString(), response);

//            Assert.Equal(response.Body.ToString(),
//                $@"<?xml version=""1.0"" encoding=""utf-8""?>
//<D:multistatus xmlns:D=""DAV:"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
//  <D:response>
//    <D:href>/api/v1/caldav/foo@gmail.com/calendars/Foocollection/</D:href>
//    <D:propstat>
//      <D:status>HTTP/1.1 424 Failed Dependency</D:status>
//      <D:prop>
//        <C:calendar-test />
//      </D:prop>
//    </D:propstat>
//    <D:propstat>
//      <D:status>HTTP/1.1 403 Forbidden</D:status>
//      <D:prop>
//        <C:calendar-description />
//      </D:prop>
//    </D:propstat>
//    <D:propstat>
//      <D:status>HTTP/1.1 424 Failed Dependency</D:status>
//      <D:prop>
//        <C:calendar-test />
//      </D:prop>
//    </D:propstat>
//  </D:response>
//</D:multistatus>");
//            using (db = new CalDavContext())
//            {
//                var testCollection = db.GetCollection("foo@gmail.com", "Foocollection");
//                var testPropperty = testCollection.Properties
//                    .SingleOrDefault(x => x.Name == "calendar-test" && x.Namespace == NamespacesSimple["C"]);

//                Assert.Null(testPropperty);
//            }
//        }
//    }
//}