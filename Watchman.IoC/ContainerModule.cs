using Autofac;
using Watchman.IoC.Modules;

namespace Watchman.IoC
{
    public class ContainerModule
    {
        private readonly string _connectionString;

        public ContainerModule(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public ContainerBuilder GetBuilder()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new DatabaseModule(this._connectionString));
            builder.RegisterModule<CommandModule>();
            builder.RegisterModule<QueryModule>();
            builder.RegisterModule<ServiceModule>();
            builder.RegisterModule<ControllerModule>();
            return builder;
        }
    }
}
