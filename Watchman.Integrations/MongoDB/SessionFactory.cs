using MongoDB.Driver;

namespace Watchman.Integrations.MongoDB
{
    public class SessionFactory : ISessionFactory
    {
        private readonly IMongoDatabase _database;

        public SessionFactory(IMongoDatabase database)
        {
            _database = database;
        }

        public ISession Create()
        {
            return new Session(_database);
        }
    }
}
