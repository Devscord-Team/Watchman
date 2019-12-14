using System;
using System.Collections.Generic;
using System.Text;
using Watchman.DomainModel.DiscordServer;
using Watchman.Integrations.MongoDB;

namespace Watchman.Discord
{
    class HelpGenerator
    {
        private readonly ISession _session;

        public HelpGenerator(ISessionFactory sessionFactory)
        {
            _session = sessionFactory.Create();
            //_session.Get<Role>()
        }

    }
}
