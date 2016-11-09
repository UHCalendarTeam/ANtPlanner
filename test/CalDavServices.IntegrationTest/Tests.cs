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
using DataLayer.Repositories.Implementations;
using DataLayer.Repositories;

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
            #region User
            var user = new User("admin", "admin@admin.uh.cu", "1234");
            #endregion

            #region Principal

            var displayName = PropertyCreation.CreateProperty("displayname", "D", user.DisplayName);

            var principal = new Principal("admin@admin.uh.cu", SystemProperties.PrincipalType.User);
            #endregion

            #region resource
            var resources = new List<CalendarResource>
                        {
                            new CalendarResource("/collections/groups/public/C212/test.ics", "test.ics")

                        };
            #endregion            

            #region CalendarHome
            var calHome = CalendarHomeRepository.CreateCalendarHome(principal);
            
            
            //var homeCollection = new CalendarHome("/collections/groups/public/", "PubicCollections");
            #endregion
            
            user.Principal = principal;

            var calHomeSet = PropertyCreation.CreateCalHomeSetWithHref(calHome.Url);
                        
            principal.Properties.Add(calHomeSet);

            principal.CalendarHome = calHome;

            //principal.CalendarCollections = collection;



            //user.Resources = resources;
            _context.Users.Add(user);
            _context.Principals.Add(principal);
            _context.CalendarHomeCollections.Add(calHome);
            _context.CalendarCollections.AddRange(calHome.CalendarCollections);
            await _context.SaveChangesAsync();

            var collectionRepo = new CollectionRepository(_context);

            var collectionC212 = collectionRepo.Get("/collections/groups/public/C212/");

            collectionC212.CalendarResources.Add(resources[0]);

            await _context.SaveChangesAsync();


            #endregion
        }
    }
}
