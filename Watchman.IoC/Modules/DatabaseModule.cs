using Autofac;
using LiteDB;
using MongoDB.Driver;
using System.Reflection;
using Watchman.Integrations.Database;

namespace Watchman.IoC.Modules
{
    public class DatabaseModule : Autofac.Module
    {
        private readonly string _mongoConnectionString;
        private readonly string _liteConnectionString;

        public DatabaseModule(string mongoConnectionString, string liteConnectionString)
        {
            this._mongoConnectionString = mongoConnectionString;
            this._liteConnectionString = liteConnectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var assembly = typeof(DatabaseModule)
                .GetTypeInfo()
                .Assembly;

            builder.Register((c, p) => new MongoClient(this._mongoConnectionString).GetDatabase("devscord"))
                .As<IMongoDatabase>()
                .SingleInstance();
            builder.Register((c, p) => new LiteDatabase(this._liteConnectionString))
                .As<ILiteDatabase>()
                .SingleInstance();

            builder.Register((c, p) => 
            {
                var mapper = BsonMapper.Global.UseCamelCase();
                mapper.Entity<Entity>().Id(x => x.Id);
                return new LiteDatabase(this._liteConnectionString, mapper); 
            }).As<ILiteDatabase>().SingleInstance();
                
            builder.RegisterType<SessionFactory>()
                .As<ISessionFactory>()
                .SingleInstance();
        }
    }
}
