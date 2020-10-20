using System.Linq;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Configuration
{
    public class GetConfigurationQueryHandler : IQueryHandler<GetConfigurationQuery, GetConfigurationQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetConfigurationQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetConfigurationQueryResult Handle(GetConfigurationQuery query)
        {
            using var session = this._sessionFactory.Create();
            var allConfigurationItems = session.Get<ConfigurationItem>();
            var serverConfigurationItems = allConfigurationItems.Where(x => x.ServerId == query.ServerId);
            var defaultConfigurationItems = allConfigurationItems.Where(x => x.ServerId == 0);
            if (!serverConfigurationItems.Any())
            {
                return new GetConfigurationQueryResult(defaultConfigurationItems);
            }
            foreach (var configurationItem in defaultConfigurationItems)
            {
                if (serverConfigurationItems.Any(x => x.Name == configurationItem.Name))
                {
                    continue;
                }
                serverConfigurationItems.Append(configurationItem);
            }
            return new GetConfigurationQueryResult(serverConfigurationItems);
        }
    }
}