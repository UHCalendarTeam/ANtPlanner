using System.Net.Http;
using System.Threading.Tasks;
using CalDav_Services;
using DataLayer;
using DataLayer.Models.ACL;
using DataLayer.Models.Entities;
using Microsoft.AspNet.TestHost;
using Xunit;

namespace IntegrationTests
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class ACLReportTests
    {
        private readonly HttpClient _client;

        private readonly TestServer _server;

        public ACLReportTests()
        {
            // Arrange
            _server = new TestServer(TestServer.CreateBuilder()
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        private void InitializeDB()
        {
            var context = new CalDavContext();
            ///create the principal
            var principal = new Principal
            {
                //Displayname = "Adriano",
                PrincipalURL =
                    @"<D:href xmlns:D=""DAV:"">http://www.example.com/acl/user/a.flechilla91@gmail.com</D:href>"
            };
            ///create the owner property for the principal
            var displayNameProp = new Property("displayname", "DAV:")
            {
                Value = @"<D:displayname xmlns:D=""DAV:"">Adriano</D:displayname> "
            };
            principal.Properties.Add(displayNameProp);

            ///create the acl property
            var aclProperty = new Property
            {
                Name = "acl",
                Namespace = "DAV:",
                Value = @"<D:acl xmlns:D=""DAV:\"">
<D:ace>
 <D:principal>
  <D:href>http://www.example.com/acl/user/a.flechilla91@gmail.com</D:href>
</D:principal>
 <D:grant>
  <D:privilege>< D:write/></D:privilege>
       </D:grant>
        </D:ace>
         </D:acl> "
            };

            principal.Properties.Add(aclProperty);

            ///create a calendarResource entry in the DB
            var calRes = new CalendarResource("http://example.com/resources/example1", "example1");
            calRes.Properties.Add(aclProperty);
            context.SaveChanges();
        }

        /// <summary>
        ///     Testint the ACLReport.AclPrincipalPropSet
        /// </summary>
        [Fact]
        public async Task UnitTest1()
        {
            ///initialize the db with necessary data
            InitializeDB();
            var acl = new ACLReportTests();

            var response = await sendRequest();

            Assert.NotNull(response);
        }

        private async Task<string> sendRequest(string method = "profind", string uri = "http://localhost/api/v1/caldav")
        {
            var response = await _client.SendAsync(new HttpRequestMessage(new HttpMethod(method), uri));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }

    internal class MockHelper
    {
    }
}