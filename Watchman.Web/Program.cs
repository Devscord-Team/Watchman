using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Watchman.Integrations.MongoDB;
using Watchman.Web.ServiceProviders;

namespace Watchman.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MongoConfiguration.Initialize();
            var configuration = GetConfiguration();
            CreateHostBuilder(args, configuration).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration)
        {
            return Host.CreateDefaultBuilder(args)
.UseServiceProviderFactory(new AutofacServiceProviderFactory(configuration))
.ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder.UseStartup<Startup>();
});
        }

        public static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
#if !DEBUG
                .AddJsonFile("appsettings.Production.json", optional: false)
#else
                .AddJsonFile("appsettings.Development.json", optional: false)
#endif
                .AddEnvironmentVariables();
            return builder.Build();
        }
    }
}
