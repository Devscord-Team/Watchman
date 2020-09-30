using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Settings
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
            var configurationItems = allConfigurationItems.Where(x => x.ServerId == query.ServerId);
            var defaultConfigurationItems = allConfigurationItems.Where(x => x.ServerId == 0);
            if (!configurationItems.Any())
            {
                return new GetConfigurationQueryResult(defaultConfigurationItems);
            }
            foreach (var configurationItem in defaultConfigurationItems)
            {
                if (configurationItems.FirstOrDefault(x => x.Name == configurationItem.Name) != null)
                {
                    continue;
                }
                configurationItems.Append(configurationItem);
            }
                
            return new GetConfigurationQueryResult(configurationItems);
        }
    }
}