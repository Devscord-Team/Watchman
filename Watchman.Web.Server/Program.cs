using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Watchman.Discord;
using Watchman.Integrations.MongoDB;

namespace Watchman.Web.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MongoConfiguration.Initialize();
            var configuration = GetConfiguration();
            RunWatchmanBot(configuration);
            CreateHostBuilder(args, configuration).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory(x => ConfigureAutofac(x, configuration)))
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

        public static void RunWatchmanBot(IConfiguration configuration)
        {
            var discordConfiguration = new DiscordConfiguration
            {
                Token = configuration["Discord:Token"],
                MongoDbConnectionString = configuration.GetConnectionString("Mongo")
            };
            var bot = new WatchmanBot(discordConfiguration, new Watchman.IoC.ContainerModule(configuration.GetConnectionString("Mongo")).GetBuilder().Build()); //todo optimalize to single instance of IoC
            Task.Factory.StartNew(() => bot.Start(), TaskCreationOptions.LongRunning);
        }
        private static void ConfigureAutofac(ContainerBuilder builder, IConfiguration configuration)
        {
            var container = new Watchman.IoC.ContainerModule(configuration.GetConnectionString("Mongo"));
            builder = container.GetBuilder();
        }
    }
}
