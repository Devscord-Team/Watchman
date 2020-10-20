using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using Watchman.Discord;
using Watchman.IoC;
using Watchman.Web.Jobs;

namespace Watchman.Web.ServiceProviders
{
    public class AutofacServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        private readonly IConfiguration _configuration;

        public AutofacServiceProviderFactory(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var containerModule = new ContainerModule(this._configuration.GetConnectionString("Mongo"), this._configuration.GetConnectionString("Lite"));
            var builder = containerModule.GetBuilder();
            this.RegisterCustomServices(builder);
            builder.Populate(services);
            return builder;
        }

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            var container = containerBuilder.Build();
            var workflowBuilder = new WatchmanBot(new DiscordConfiguration
            {
                MongoDbConnectionString = this._configuration.GetConnectionString("Mongo"),
                Token = this._configuration["Discord:Token"],
                ExceptionChannelId = ulong.Parse(this._configuration["Logging:ExceptionChannelId"]),
                ExceptionServerId = ulong.Parse(this._configuration["Logging:ExceptionServerId"])
            }, container.Resolve<IComponentContext>()).GetWorkflowBuilder();

            workflowBuilder.Build();
            container.Resolve<HangfireJobsService>().SetDefaultJobs(container);
            return new AutofacServiceProvider(container);
        }

        private void RegisterCustomServices(ContainerBuilder builder)
        {
            var jobs = Assembly.GetAssembly(typeof(AutofacServiceProviderFactory)).GetTypes()
                .Where(x => x.IsAssignableTo<IHangfireJob>() && !x.IsInterface).ToList();
            foreach (var job in jobs)
            {
                builder.RegisterType(job).AsSelf().PreserveExistingDefaults().SingleInstance();
            }
        }
    }
}
