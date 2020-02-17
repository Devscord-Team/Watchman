using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using RestSharp;
using System.Net.Http;

namespace Watchman.Web.Server.Tests.Integration
{
    public class Configurator
    {
        private readonly TestServer server;
        private readonly RestClient client;

        public Configurator()
        {
            server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            client = new RestClient(server.BaseAddress);
        }

        public RestClient GetClient()
        {
            return client;
        }
    }
}
