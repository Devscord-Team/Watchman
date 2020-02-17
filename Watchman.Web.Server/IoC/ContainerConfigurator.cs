using Autofac;
using Watchman.Web.Server.IoC.Modules;

namespace Watchman.Web.Server.IoC
{
    public static class ContainerConfigurator
    {
        public static void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<DatabaseModule>();
            builder.RegisterModule<CommandModule>();
            builder.RegisterModule<QueryModule>();
            builder.RegisterModule<ServiceModule>();
        }
    }
}
