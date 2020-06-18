using MongoDB.Driver;

namespace Watchman.Integrations.MongoDB
{
    public class SessionFactory : ISessionFactory
    {
        private readonly IMongoDatabase _database;

        public SessionFactory(IMongoDatabase database) => this._database = database;

        public ISession Create() => new Session(this._database);
    }
}
