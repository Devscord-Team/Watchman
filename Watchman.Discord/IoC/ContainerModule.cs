using Autofac;
using Watchman.Discord.Ioc.Modules;
using Watchman.Discord.IoC.Modules;

namespace Watchman.Discord.Ioc
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
            builder.RegisterModule<ServiceModule>();
            builder.RegisterModule<CommandModule>();
            builder.RegisterModule<QueryModule>();
            builder.RegisterModule<ControllerModule>();
            return builder;
        }
    }
}
