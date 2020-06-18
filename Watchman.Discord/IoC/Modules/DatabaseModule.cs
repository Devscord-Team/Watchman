using Autofac;
using MongoDB.Driver;
using System.Reflection;
using Watchman.Integrations.MongoDB;

namespace Watchman.Discord.IoC.Modules
{
    public class DatabaseModule : Autofac.Module
    {
        private readonly DiscordConfiguration configuration;

        public DatabaseModule(DiscordConfiguration configuration) => this.configuration = configuration;

        protected override void Load(ContainerBuilder builder)
        {
            var assembly = typeof(DatabaseModule)
                .GetTypeInfo()
                .Assembly;

            builder.Register((c, p) => new MongoClient(this.configuration.MongoDbConnectionString).GetDatabase("devscord"))
                .As<IMongoDatabase>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SessionFactory>()
                .As<ISessionFactory>()
                .InstancePerLifetimeScope();
        }
    }
}
