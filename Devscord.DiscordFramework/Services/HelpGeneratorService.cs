using Devscord.DiscordFramework.Services;
using Watchman.Integrations.MongoDB;

namespace Watchman.Discord
{
    class HelpGenerator : IService
    {
        private readonly ISession _session;

        public HelpGenerator(ISessionFactory sessionFactory)
        {
            _session = sessionFactory.Create();
            //_session.Get<Role>()
        }
    }
}
