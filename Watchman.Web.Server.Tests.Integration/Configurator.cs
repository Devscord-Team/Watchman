using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RestSharp;
using Watchman.Cqrs;
using Watchman.Web.Server.IoC;

namespace Watchman.Web.Server.Tests.Integration
{
    public static class Configurator
    {
        private static TestServer server;
        private static object spinLock = new object();

        public static RestClient GetClient()
        {
            lock (spinLock)
            {
                var webHostBuilder = new WebHostBuilder()
                    .ConfigureTestContainer<ContainerBuilder>(x => ContainerConfigurator.ConfigureContainer(x)) 
                    .ConfigureServices(services => services.AddAutofac())
                    .UseStartup<Startup>();
                server = new TestServer(webHostBuilder);
                var test = server.Host.Services.GetRequiredService<IQueryBus>();
            }

            var client = server.CreateClient();
            //var test = client.GetAsync("/Responses/GetResponses").Result;
            return new RestClient(server.BaseAddress);
        }
    }
}
