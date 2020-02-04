using Autofac;
using Watchman.ScheduleRunner.IoC.Modules;

namespace Watchman.ScheduleRunner.IoC
{
    public class ContainerModule
    {
        public ContainerBuilder GetBuilder()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<DatabaseModule>();
            builder.RegisterModule<CommandModule>();
            builder.RegisterModule<QueryModule>();
            builder.RegisterModule<ServiceModule>();
            return builder;
        }
    }
}
