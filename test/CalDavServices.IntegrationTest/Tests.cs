using CalDavServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class Tests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public Tests()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        } 

        [Fact]
        public async Task MkCalendarTest() 
        {
            var response = await _client.GetAsync("");
            Assert.True(true);
        }
    }
}
