using Autofac;
using Watchman.Discord.Ioc.Modules;

namespace Watchman.Discord.Ioc
{
    public class ContainerModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<CommandModule>();
            builder.RegisterModule<QueryModule>();
        }
    }
}
