using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Settings
{
    public class GetConfigurationInformationQueryHandler : IQueryHandler<GetConfigurationInformationQuery,
        GetConfigurationInformationQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetConfigurationInformationQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetConfigurationInformationQueryResult Handle(GetConfigurationInformationQuery query)
        {
            using var session = this._sessionFactory.Create();
            var configurationItem = session.Get<ConfigurationItem>();
            return new GetConfigurationInformationQueryResult(configurationItem);
        }
    }
}