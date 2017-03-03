//todo:remove after resolve dependecy using CalDavServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;

namespace Tests
{
    public class Tests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        //private readonly CalDavContext _context;

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

        public Tests()
        {
            //_client = fixture.Client;      
            _server = new TestServer(new WebHostBuilder()
                                    //todo:remove after resolve dependecy    .UseStartup<TestStartup>()
                                    .UseEnvironment("Development"));
            _client = _server.CreateClient();
           // _context = new CalDavContext();
            //var controller = new CalDavController();
        }

         [Fact]
        public async Task MkCalendarTest()
        {
            var response = await _client.GetAsync("");
            //todo:remove after resolve dependecy Assert.True(true);
        }

         [Fact]
        public async Task PropfindPropnameTest()
        {           
            var request = new HttpRequestMessage(new HttpMethod("PROPFIND"), "/collections/groups/public/C212/");
            request.Content = new StringContent(@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:propfind xmlns:D=""DAV:"">
   <propname/>
</D:propfind>");
            request.Headers.Add("Authorization", "Basic YWRtaW5AYWRtaW4udWguY3U6YWRtaW4=");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

            var response = await _client.SendAsync(request);
            //todo:remove after resolve dependecy  Assert.Equal(response.StatusCode, (HttpStatusCode)207);

        }
    }
}
