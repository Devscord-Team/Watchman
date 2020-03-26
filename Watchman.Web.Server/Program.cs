using Autofac;
using Autofac.Builder;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Watchman.Discord;
using Watchman.Discord.IoC.Modules;
using Watchman.Integrations.MongoDB;
using Watchman.IoC.Modules;

namespace Watchman.Web.Server
{
    public class Program
    {
        public static IConfiguration Configuration { get; set; }
        public static IContainer Container { get; set; }
        public static ContainerBuilder Builder { get; set; } = new ContainerBuilder();

        public static void Main(string[] args)
        {
            MongoConfiguration.Initialize();

            Configuration = GetConfiguration();
            
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacProviderFactory())
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
                .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
#endif
                .AddEnvironmentVariables();
            return builder.Build();
        }
    }

    public class AutofacProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            RegisterModules(Program.Builder,
                new Watchman.IoC.Modules.DatabaseModule(Program.Configuration.GetConnectionString("Mongo")),
                new Watchman.IoC.Modules.CommandModule(),
                new Watchman.IoC.Modules.QueryModule(),
                new Watchman.IoC.Modules.ServiceModule(),
                new Watchman.IoC.Modules.ControllerModule(),
                new Watchman.Web.Server.IoC.ServerServiceModule());
            Program.Builder.Populate(services);
            return Program.Builder;
        }

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            Program.Container = Program.Builder.Build();
            RunWatchmanBot(Program.Configuration, Program.Container);
            return new AutofacProvider(Program.Container);
        }

        private static void RegisterModules(ContainerBuilder builder, params Autofac.Module[] modules)
        {
            var method = typeof(Module).GetMethod("Load", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            foreach (var module in modules)
            {
                method.Invoke(module, new object[] { builder });
            }
        }

        public void RunWatchmanBot(IConfiguration configuration, IContainer container)
        {
            var discordConfiguration = new DiscordConfiguration
            {
                Token = configuration["Discord:Token"],
                MongoDbConnectionString = configuration.GetConnectionString("Mongo")
            };
            var workflowBuilder = new WatchmanBot(discordConfiguration, container).GetWorkflowBuilder();
        }
    }

    public class AutofacProvider : IServiceProvider
    {
        private readonly IContainer _container;

        public AutofacProvider(IContainer container)
        {
            this._container = container;
        }

        public object GetService(Type serviceType)
        {
            return this._container.Resolve(serviceType);
        }
    }
}
