using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Watchman.Discord;
using Watchman.IoC;

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
            var containerModule = new ContainerModule(this._configuration.GetConnectionString("Mongo"));
            var builder = containerModule.GetBuilder();
            builder.Populate(services);
            return builder;
        }

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            var container = containerBuilder.Build();

            var workflowBuilder = new WatchmanBot(new DiscordConfiguration
            {
                MongoDbConnectionString = this._configuration.GetConnectionString("Mongo"),
                Token = this._configuration["Discord:Token"]
            }, container.Resolve<IComponentContext>()).GetWorkflowBuilder();

            workflowBuilder.Build();
            container.Resolve<HangfireJobsService>().SetDefaultJobs(container);
            return new AutofacServiceProvider(container);
        }
    }
}
