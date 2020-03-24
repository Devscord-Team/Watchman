using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Watchman.Discord;
using Watchman.Discord.IoC.Modules;
using Watchman.Integrations.MongoDB;
using Watchman.IoC.Modules;

namespace Watchman.Web.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MongoConfiguration.Initialize();

            var configuration = GetConfiguration();
            var containerBuilder = new ContainerBuilder();
            ConfigureAutofac(containerBuilder, configuration); //todo optimalize to single instance of IoC
            var container = containerBuilder.Build();
            
            RunWatchmanBot(configuration, container);
            var builded = CreateHostBuilder(args, container).Build();
            builded.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IContainer container) =>
            Host.CreateDefaultBuilder(args)
            
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

        public static void RunWatchmanBot(IConfiguration configuration, IContainer container)
        {
            var discordConfiguration = new DiscordConfiguration
            {
                Token = configuration["Discord:Token"],
                MongoDbConnectionString = configuration.GetConnectionString("Mongo")
            };
            var workflowBuilder = new WatchmanBot(discordConfiguration, container).GetWorkflowBuilder();
            Task.Run(() => workflowBuilder.Run());
        }
        private static void ConfigureAutofac(ContainerBuilder builder, IConfiguration configuration)
        {
            RegisterModules(builder,
                new Watchman.IoC.Modules.DatabaseModule(configuration.GetConnectionString("Mongo")),
                new Watchman.IoC.Modules.CommandModule(),
                new Watchman.IoC.Modules.QueryModule(),
                new Watchman.IoC.Modules.ServiceModule(),
                new Watchman.IoC.Modules.ControllerModule(),
                new Watchman.Web.Server.IoC.ServerServiceModule());
        }

        private static void RegisterModules(ContainerBuilder builder, params Autofac.Module[] modules)
        {
            var method = typeof(Module).GetMethod("Load", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            foreach (var module in modules)
            {
                method.Invoke(module, new object[] { builder });
            }
        }
    }
}
