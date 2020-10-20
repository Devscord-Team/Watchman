using Autofac;
using LiteDB;
using MongoDB.Driver;
<<<<<<< HEAD
using System.Reflection;
using Watchman.Integrations.Database;
=======
using Watchman.Integrations.MongoDB;
>>>>>>> master

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
<<<<<<< HEAD
            var assembly = typeof(DatabaseModule)
                .GetTypeInfo()
                .Assembly;

            builder.Register((c, p) => new MongoClient(this._mongoConnectionString).GetDatabase("devscord"))
=======
            builder.Register((c, p) => new MongoClient(this._connectionString).GetDatabase("devscord"))
>>>>>>> master
                .As<IMongoDatabase>()
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
