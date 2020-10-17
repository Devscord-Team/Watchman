using LiteDB;

using MongoDB.Driver;
using Watchman.Integrations.Database.LiteDB;
using Watchman.Integrations.Database.MongoDB;

namespace Watchman.Integrations.Database
{
    public class SessionFactory : ISessionFactory
    {
        private readonly IMongoDatabase mongoDatabase;
        private readonly ILiteDatabase liteDatabase;

        public SessionFactory(IMongoDatabase mongoDatabase, ILiteDatabase liteDatabase)
        {
            this.mongoDatabase = mongoDatabase;
            this.liteDatabase = liteDatabase;
        }

        public ISession CreateMongo()
        {
            return new MongoSession(this.mongoDatabase);
        }

        public ISession CreateLite()
        {
            return new LiteSession(this.liteDatabase);
        }
    }
}
