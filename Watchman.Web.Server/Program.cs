using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Watchman.Integrations.MongoDB;
using Watchman.Web.Server.ServiceProviders;

namespace Watchman.Web.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MongoConfiguration.Initialize();
            var configuration = GetConfiguration();
            CreateHostBuilder(args, configuration).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory(configuration))
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

        public static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
#if RELEASE
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
#else
                .AddJsonFile("appsettings.Development.json", optional: true)
#endif
                .AddEnvironmentVariables();
            return builder.Build();
        }
    }
}
