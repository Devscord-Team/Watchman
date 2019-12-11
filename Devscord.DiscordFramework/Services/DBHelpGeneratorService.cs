using Devscord.DiscordFramework.Services;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Help.Queries;
using Watchman.Integrations.MongoDB;

namespace Devscord.DiscordFramework.Services
{
    class DbHelpGenerator : IService
    {
        private readonly ISession _session;

        public DbHelpGenerator(ISessionFactory sessionFactory)
        {
            
        }

        private Task GenerateDefaultHelpDB()
        {

        }
    }
}
