using Autofac;
using Watchman.IoC.Modules;

namespace Watchman.IoC
{
    public class ContainerModule
    {
        private readonly DiscordConfiguration configuration;

        public ContainerModule(DiscordConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public ContainerBuilder GetBuilder()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new DatabaseModule(configuration));
            builder.RegisterModule<CommandModule>();
            builder.RegisterModule<QueryModule>();
            builder.RegisterModule<ServiceModule>();
            builder.RegisterModule<ControllerModule>();
            return builder;
        }
    }
}
