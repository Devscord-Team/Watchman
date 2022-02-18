using Autofac;
using LiteDB;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Watchman.Integrations.Database;

namespace Watchman.IoC.Modules
{
    [ExcludeFromCodeCoverage]
    public class DatabaseModule : Autofac.Module
    {
        private readonly string mongoConnectionString;
        private readonly string liteConnectionString;

        public DatabaseModule(string mongoConnectionString, string liteConnectionString)
        {
            this.mongoConnectionString = mongoConnectionString;
            this.liteConnectionString = liteConnectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var assembly = typeof(DatabaseModule)
                .GetTypeInfo()
                .Assembly;

            if(!string.IsNullOrWhiteSpace(this.mongoConnectionString))
            {
                builder.Register((c, p) => new MongoClient(this.mongoConnectionString).GetDatabase("devscord"))
                    .As<IMongoDatabase>()
                    .SingleInstance();
            }
            if (!string.IsNullOrWhiteSpace(this.liteConnectionString))
            {
                builder.Register((c, p) => new LiteDatabase(this.liteConnectionString))
                    .As<ILiteDatabase>()
                    .SingleInstance();
                builder.Register((c, p) =>
                {
                    var mapper = BsonMapper.Global.UseCamelCase();
                    mapper.Entity<Entity>().Id(x => x.Id);
                    return new LiteDatabase(this.liteConnectionString, mapper);
                }).As<ILiteDatabase>().SingleInstance();
            }

            builder.RegisterType<SessionFactory>()
                .As<ISessionFactory>()
                .PreserveExistingDefaults()
                .SingleInstance();
        }
    }
}
