using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Integrations.Database;

namespace Watchman.Discord.IntegrationTests.TestEnvironment.FakeDatabases
{
    internal class FakeSessionFactory : ISessionFactory
    {
        public ISession CreateLite()
        {
            throw new NotImplementedException();
        }

        public ISession CreateMongo()
        {
            throw new NotImplementedException();
        }
    }
}
