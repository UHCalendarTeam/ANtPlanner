using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using CalDAV.Core;
using DataLayer.Entities;
using Microsoft.Data.Entity;
using TreeForXml;
using Xunit;

namespace CalDav_tests
{
    public class ProppatchTest
    {
        private Dictionary<string, string> Namespaces = new Dictionary<string, string> { { "D", @"xmlns:D=""DAV:""" }, { "C", @"xmlns:C=""urn:ietf:params:xml:ns:caldav""" } };
        private Dictionary<string, string> NamespacesSimple = new Dictionary<string, string> { { "D", "DAV:" }, { "C", "urn:ietf:params:xml:ns:caldav" } };

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

        [Fact]
        public void ModifyCollectionPropertySuccesful()
        {
            var db = MockDatabase();
            var fs = new FileSystemManagement();

            var caldav = new CalDav(fs, db);
            var propertiesAndHeaders = new Dictionary<string,string> { {"userEmail", "foo@gmail.com" } , {"collectionName" ,"Foocollection" }  };
            var body = XDocument.Parse($@"<propertyupdate {Namespaces["D"]} {Namespaces["C"]}>
  <set>
    <prop>
      <C:calendar-description>void description</C:calendar-description>
    </prop>
  </set>
</propertyupdate>");

            var request = XmlTreeStructure.Parse(body.ToString());

            var response = caldav.PropPatch(propertiesAndHeaders, request.ToString());

            Assert.Equal(response, "");
        }

        [Fact]
        public void CreateSuccesful()
        {
            
        }

        [Fact]
        public void CreateAndThenRemoveSuccesful()
        {
            
        }

        [Fact]
        public void SeveralSetAndRemoveSuccesful()
        {
            
        }

        [Fact]
        public void ErrorCoctel()
        {
            
        }
    }
}
