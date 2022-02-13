using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;
using Watchman.Integrations.Database.MongoDB;
using Watchman.Web.ServiceProviders;

namespace Watchman.Web
{
    [ExcludeFromCodeCoverage]
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
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseConfiguration(configuration).UseStartup<Startup>());
        }

        public static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
#if !DEBUG
                .AddJsonFile("appsettings.json", optional: false)
#else
                .AddJsonFile("appsettings.Development.json", optional: false)
#endif
                .AddEnvironmentVariables();
            return builder.Build();
        }
    }
}
