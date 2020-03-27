using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Discord;
using Watchman.IoC;

namespace Watchman.Web.Server.ServiceProviders
{
    public class AutofacServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        private readonly IConfiguration configuration;

        public AutofacServiceProviderFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var containerModule = new ContainerModule(configuration.GetConnectionString("Mongo"));
            var builder = containerModule.GetBuilder();
            builder.Populate(services);
            return builder;
        }

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            var container = containerBuilder.Build();
            var configuration = container.Resolve<IConfiguration>();

            var watchman = new WatchmanBot(new DiscordConfiguration 
            { 
                MongoDbConnectionString = configuration.GetConnectionString("Mongo"), 
                Token = configuration["Discord:Token"] 
            }, container.Resolve<IComponentContext>()).GetWorkflowBuilder();

            return new AutofacServiceProvider(container);
        }
    }

    public class AutofacServiceProvider : IServiceProvider
    {
        private readonly IContainer container;

        public AutofacServiceProvider(IContainer container)
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            return this.container.Resolve(serviceType);
        }
    }
}
