using Autofac;
using MongoDB.Driver;
using Watchman.Integrations.MongoDB;

namespace Watchman.IoC.Modules
{
    public class DatabaseModule : Autofac.Module
    {
        private readonly string _connectionString;

        public DatabaseModule(string connectionString)
        {
            this._connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) => new MongoClient(this._connectionString).GetDatabase("devscord"))
                .As<IMongoDatabase>()
                .SingleInstance();

            builder.RegisterType<SessionFactory>()
                .As<ISessionFactory>()
                .SingleInstance();
        }
    }
}
