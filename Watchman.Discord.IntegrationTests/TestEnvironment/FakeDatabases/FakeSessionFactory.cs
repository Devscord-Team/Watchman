using System.Text;
using Watchman.Integrations.Database;

namespace Watchman.Discord.IntegrationTests.TestEnvironment.FakeDatabases
{
    internal class FakeSessionFactory : ISessionFactory
    {
        public ISession CreateLite()
        {
            return new FakeSession();
        }

        public ISession CreateMongo()
        {
            return new FakeSession();
        }
    }
}
