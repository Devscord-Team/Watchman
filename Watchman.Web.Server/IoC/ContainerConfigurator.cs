using Autofac;
using Microsoft.Extensions.Configuration;
using Watchman.Web.Server.IoC.Modules;

namespace Watchman.Web.Server.IoC
{
    public static class ContainerConfigurator
    {
        public static void ConfigureContainer(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterModule(new DatabaseModule(configuration));
            builder.RegisterModule<CommandModule>();
            builder.RegisterModule<QueryModule>();
            builder.RegisterModule<ServiceModule>();
        }
    }
}
