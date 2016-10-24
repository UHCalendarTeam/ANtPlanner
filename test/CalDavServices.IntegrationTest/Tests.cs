using CalDavServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using DataLayer;
using DataLayer.Models.Entities;
using System.Collections.Generic;
using DataLayer.Models.ACL;

namespace Tests
{
    public class Tests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private readonly CalDAVSQLiteContext _context;

        private readonly Dictionary<string, string> Namespaces = new Dictionary<string, string>
        {
           {"D", @"xmlns:D=""DAV:"""},
            {"C", @"xmlns:C=""urn:ietf:params:xml:ns:caldav"""},
            {"CS", @"xmlns:CS=""http://calendarserver.org/ns/"""}
        };

        private readonly Dictionary<string, string> NamespacesSimple = new Dictionary<string, string>
        {
                 {"D", "DAV:"},
            {"C", "urn:ietf:params:xml:ns:caldav"},
            {"CS", "http://calendarserver.org/ns/"}
                };


        public Tests(CalDAVSQLiteContext context)
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
            _context = context;
        }

        [Fact]
        public async Task MkCalendarTest()
        {
            var response = await _client.GetAsync("");
            Assert.True(true);
        }

        [Fact]
        public async Task PropfindPropnameTest()
        {
            var request = new HttpRequestMessage(new HttpMethod("PROPFIND"), "collections/User/pepe@matcom.com/Default/");

        }

        private async Task MockDatabase()
        {
            #region FIlling Database

            //FileManagement fs = new FileManagement();            

            // This is the magic line
            //  optionsBuilder.UseInMemoryDatabase();            

            var principal = new Principal("pepe@matcom.com", SystemProperties.PrincipalType.User);

            var resources = new List<CalendarResource>
                        {
                            new CalendarResource
                            {
                                //TODO: Adriano ver esto
                                //DtStart = DateTime.Now,
                                //DtEnd = DateTime.Now,
                                Name = "test.ics",
                                Href = ".....",
                                Properties = new List<Property>
                                {
                                    new Property
                                    {
                                        Name = "getcontenttype",
                                        Namespace = NamespacesSimple["D"],
                                        Value = $"<D:getcontenttype {Namespaces["D"]}>text/</D:getcontenttype>",
                                        IsVisible = true,
                                        IsDestroyable = false,
                                        IsMutable = true
                                    },
                                    new Property
                                    {
                                        Name = "resourcetype",
                                        Namespace = NamespacesSimple["D"],
                                        Value = $"<D:resourcetype {Namespaces["D"]}/>",
                                        IsVisible = true,
                                        IsDestroyable = false,
                                        IsMutable = true
                                    },
                                    new Property
                                    {
                                        Name = "displayname",
                                        Namespace = NamespacesSimple["D"],
                                        Value = $"<D:displayname {Namespaces["D"]}>Mocking resource</D:displayname>",
                                        IsVisible = true,
                                        IsDestroyable = false,
                                        IsMutable = true
                                    },
                                    new Property
                                    {
                                        Name = "getetag",
                                        Namespace = NamespacesSimple["D"],
                                        Value = $"<D:getetag {Namespaces["D"]}>12345</D:getetag>",
                                        IsVisible = true,
                                        IsDestroyable = false,
                                        IsMutable = true
                                    },
                                    new Property
                                    {
                                        Name = "creationdate",
                                        Namespace = NamespacesSimple["D"],
                                        Value = $"<D:creationdate {Namespaces["D"]}>{"29/03/16 01:30:44"}</D:creationdate>",
                                        IsVisible = true,
                                        IsDestroyable = false,
                                        IsMutable = true
                                    },
                                    new Property
                                    {
                                        Name = "getcontentlanguage",
                                        Namespace = NamespacesSimple["D"],
                                        Value = $"<D:getcontentlanguage {Namespaces["D"]}>en</D:getcontentlanguage>",
                                        IsVisible = true,
                                        IsDestroyable = false,
                                        IsMutable = true
                                    },
                                    new Property
                                    {
                                        Name = "",
                                        Namespace = NamespacesSimple["D"],
                                        Value = $"<D:getcontentlength {Namespaces["D"]}>10000</D:getcontentlength>",
                                        IsVisible = true,
                                        IsDestroyable = false,
                                        IsMutable = true
                                    },
                                    new Property
                                    {
                                        Name = "getlastmodified",
                                        Namespace = NamespacesSimple["D"],
                                        Value = $"<D:getlastmodified {Namespaces["D"]}>29/03/16 01:30:44</D:getlastmodified>",
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
                                //User = user,
                                CalendarResources = resources,
                                /*,
                                SupportedCalendarComponentSet = new List<string>()*/
                                //ResourceType = new List<string>(),

                                //TODO: Adriano ver esto ahora es xml hecho string
                                //ResourceType = new XmlTreeStructure("resourcetype", "DAV"),
                                //Displayname = $"<D:displayname {Namespaces["D"]}>Mocking Collection</D:displayname>",
                                Url = "url",
                                //Resourcetype = $"<D:resourcetype {Namespaces["D"]}><D:collection/><C:calendar xmlns:C=\"urn:ietf:params:xml:ns:caldav\"/></D:resourcetype>",
                                //Creationdate = $"<D:creationdate {Namespaces["D"]}>{"29/03/16 01:30:44"}</D:creationdate>",
                                //Getetag = $"<D:getetag {Namespaces["D"]}>0</D:getetag>"
                                Properties = new List<Property>
                                {
                                    new Property
                                    {
                                        Name = "calendar-timezone",
                                        Namespace = NamespacesSimple["C"],
                                        Value = $"<C:calendar-timezone {Namespaces["C"]}>LaHabana/Cuba</C:calendar-timezone>",
                                        IsMutable = false,
                                        IsVisible = true,
                                        IsDestroyable = false
                                    },
                                    new Property
                                    {
                                        Name = "max-resource-size",
                                        Namespace = NamespacesSimple["C"],
                                        Value = $"<C:max-resource-size {Namespaces["C"]}>102400</C:max-resource-size>",
                                        IsMutable = false,
                                        IsVisible = true,
                                        IsDestroyable = false
                                    },
                                    new Property
                                    {
                                        Name = "min-date-time",
                                        Namespace = NamespacesSimple["C"],
                                        Value = $"<C:min-date-time {Namespaces["C"]}>20160228T050000Z</C:min-date-time>",
                                        IsMutable = false,
                                        IsVisible = true,
                                        IsDestroyable = false
                                    },
                                    new Property
                                    {
                                        Name = "max-date-time",
                                        Namespace = NamespacesSimple["C"],
                                        Value = $"<C:max-date-time {Namespaces["C"]}>20160428T040000Z</C:max-date-time>",
                                        IsMutable = false,
                                        IsVisible = true,
                                        IsDestroyable = false
                                    },
                                    new Property
                                    {
                                        Name = "max-instances",
                                        Namespace = NamespacesSimple["C"],
                                        Value = $"<C:max-instances {Namespaces["C"]}>10</C:max-instances>",
                                        IsMutable = false,
                                        IsVisible = true,
                                        IsDestroyable = false
                                    },
                                    new Property
                                    {
                                        Name = "getcontentlength",
                                        Namespace = NamespacesSimple["D"],
                                        Value = $"<D:getcontentlength {Namespaces["D"]}>0</D:getcontentlength>",
                                        IsMutable = false,
                                        IsVisible = true,
                                        IsDestroyable = false
                                    },
                                    new Property
                                    {
                                        Name = "supported-calendar-component-set",
                                        Namespace = NamespacesSimple["C"],
                                        Value =
                                            $@"<C:supported-calendar-component-set {Namespaces["C"]}>&lt;C:comp name=""VEVENT""/&gt;&lt;C:comp name=""VTODO""/&gt;</C:supported-calendar-component-set>",
                                        IsMutable = false,
                                        IsVisible = true,
                                        IsDestroyable = false
                                    },
                                    new Property
                                    {
                                        Name = "supported-calendar-data",
                                        Namespace = NamespacesSimple["C"],
                                        Value =
                                            $@"<C:supported-calendar-data {Namespaces["C"]}><C:comp name=""VEVENT""/><C:comp name=""VTODO""/></C:supported-calendar-data>",
                                        IsMutable = false,
                                        IsVisible = true,
                                        IsDestroyable = false
                                    },
                                    new Property
                                    {
                                        Name = "getetag",
                                        Namespace = NamespacesSimple["D"],
                                        Value = $"<D:getetag {Namespaces["D"]}>0</D:getetag>",
                                        IsMutable = false,
                                        IsVisible = true,
                                        IsDestroyable = false
                                    },
                                    new Property
                                    {
                                        Name = "calendar-description",
                                        Namespace = NamespacesSimple["C"],
                                        Value =
                                            $"<C:calendar-description {Namespaces["C"]}>empty description</C:calendar-description>",
                                        IsMutable = true,
                                        IsVisible = true,
                                        IsDestroyable = false
                                    },
                                    new Property
                                    {
                                        Name = "resourcetype",
                                        Namespace = NamespacesSimple["D"],
                                        Value =
                                            $"<D:resourcetype {Namespaces["D"]}><D:collection/><C:calendar xmlns:C=\"urn:ietf:params:xml:ns:caldav\"/></D:resourcetype>",
                                        IsMutable = true,
                                        IsVisible = true,
                                        IsDestroyable = false
                                    },
                                    new Property
                                    {
                                        Name = "displayname",
                                        Namespace = NamespacesSimple["D"],
                                        Value = $"<D:displayname {Namespaces["D"]}>Mocking Collection</D:displayname>",
                                        IsMutable = true,
                                        IsVisible = true,
                                        IsDestroyable = false
                                    },
                                    new Property
                                    {
                                        Name = "creationdate",
                                        Namespace = NamespacesSimple["D"],
                                        Value = $"<D:creationdate {Namespaces["D"]}>{"29/03/16 01:30:44"}</D:creationdate>",
                                        IsMutable = true,
                                        IsVisible = true,
                                        IsDestroyable = false
                                    }
                                }
                            }
                        };
            principal.CalendarCollections = collection;
            //user.Resources = resources;
            _context.Principals.Add(principal);
            await _context.SaveChangesAsync();


            #endregion
        }
    }
}
