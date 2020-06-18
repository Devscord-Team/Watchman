using Autofac;
using MongoDB.Driver;
using System.Reflection;
using Watchman.Integrations.MongoDB;

namespace Watchman.IoC.Modules
{
    public class DatabaseModule : Autofac.Module
    {
        private readonly string _connectionString;

        public DatabaseModule(string connectionString) => this._connectionString = connectionString;

        protected override void Load(ContainerBuilder builder)
        {
            var assembly = typeof(DatabaseModule)
                .GetTypeInfo()
                .Assembly;

            builder.Register((c, p) => new MongoClient(this._connectionString).GetDatabase("devscord"))
                .As<IMongoDatabase>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SessionFactory>()
                .As<ISessionFactory>()
                .InstancePerLifetimeScope();
        }
    }
}
