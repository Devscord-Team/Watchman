using Autofac;
using System.Diagnostics.CodeAnalysis;
using Watchman.IoC.Modules;

namespace Watchman.IoC
{
    [ExcludeFromCodeCoverage]
    public class ContainerModule
    {
        private readonly string _mongoConnectionString;
        private readonly string _liteConnectionString;

        public ContainerModule(string mongoConnectionString, string liteConnectionString)
        {
            this._mongoConnectionString = mongoConnectionString;
            this._liteConnectionString = liteConnectionString;
        }

        public ContainerBuilder GetBuilder()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new DatabaseModule(this._mongoConnectionString, this._liteConnectionString));
            builder.RegisterModule<CommandModule>();
            builder.RegisterModule<QueryModule>();
            builder.RegisterModule<ServiceModule>();
            builder.RegisterModule<ControllerModule>();
            return builder;
        }
    }
}
