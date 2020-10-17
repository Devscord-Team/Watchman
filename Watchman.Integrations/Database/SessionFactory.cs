using MongoDB.Driver;
using Watchman.Integrations.Database.LiteDB;
using Watchman.Integrations.Database.MongoDB;

namespace Watchman.Integrations.Database
{
    public class SessionFactory : ISessionFactory
    {
        private readonly IMongoDatabase _database;

        public SessionFactory(IMongoDatabase database, )
        {
            this._database = database;
        }

        public ISession CreateMongo()
        {
            return new MongoSession(this._database);
        }

        public ISession CreateLite()
        {
            return new LiteSession()
        }
    }
}
