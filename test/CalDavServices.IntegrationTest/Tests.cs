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
using System.Net;
using CalDavServices.Controllers;


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
                                    .UseStartup<TestStartup>()
                                    .UseEnvironment("Development"));
            _client = _server.CreateClient();
           // _context = new CalDavContext();
            //var controller = new CalDavController();
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
            var request = new HttpRequestMessage(new HttpMethod("PROPFIND"), "collections/groups/public/C212/");
            request.Content = new StringContent(@"<?xml version=""1.0"" encoding=""utf - 8"" ?>
<D:propfind xmlns: D = ""DAV:"">
   <propname/>
</D:propfind >");
            request.Headers.Add("Authorization", "Basic YWRtaW5AYWRtaW4udWguY3U6MTIzNA==");

            var response = _client.SendAsync(request);
            Assert.Equal(response.Result.StatusCode, (HttpStatusCode)200);

        }

    }
}
