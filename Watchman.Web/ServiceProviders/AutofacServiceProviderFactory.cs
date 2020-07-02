using Autofac;
using Autofac.Extensions.DependencyInjection;
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

<<<<<<< HEAD:Watchman.Web/ServiceProviders/AutofacServiceProviderFactory.cs
        public AutofacServiceProviderFactory(IConfiguration configuration) => this.configuration = configuration;

        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var containerModule = new ContainerModule(this.configuration.GetConnectionString("Mongo"));
=======
        public AutofacServiceProviderFactory(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var containerModule = new ContainerModule(this._configuration.GetConnectionString("Mongo"));
>>>>>>> master:Watchman.Web.Server/ServiceProviders/AutofacServiceProvider.cs
            var builder = containerModule.GetBuilder();
            builder.Populate(services);
            return builder;
        }

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            var container = containerBuilder.Build();

<<<<<<< HEAD:Watchman.Web/ServiceProviders/AutofacServiceProviderFactory.cs
            var watchman = new WatchmanBot(new DiscordConfiguration
            {
                MongoDbConnectionString = configuration.GetConnectionString("Mongo"),
                Token = configuration["Discord:Token"]
=======
            _ = new WatchmanBot(new DiscordConfiguration
            {
                MongoDbConnectionString = this._configuration.GetConnectionString("Mongo"),
                Token = this._configuration["Discord:Token"]
>>>>>>> master:Watchman.Web.Server/ServiceProviders/AutofacServiceProvider.cs
            }, container.Resolve<IComponentContext>()).GetWorkflowBuilder();

            container.Resolve<HangfireJobsService>().SetDefaultJobs(container);

            return new AutofacServiceProvider(container);
        }
    }
<<<<<<< HEAD:Watchman.Web/ServiceProviders/AutofacServiceProviderFactory.cs
=======

    public class AutofacServiceProvider : IServiceProvider
    {
        private readonly IContainer _container;

        public AutofacServiceProvider(IContainer container)
        {
            this._container = container;
        }

        public object GetService(Type serviceType)
        {
            return this._container.Resolve(serviceType);
        }
    }
>>>>>>> master:Watchman.Web.Server/ServiceProviders/AutofacServiceProvider.cs
}
