using System.Linq;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Settings.Queries.Handlers
{
    public class GetConfigurationQueryHandler : IQueryHandler<GetConfigurationQuery, GetConfigurationQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetConfigurationQueryHandler(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public GetConfigurationQueryResult Handle(GetConfigurationQuery query)
        {
            using var session = _sessionFactory.Create();
            var configuration = session.Get<Configuration>().FirstOrDefault() ?? Configuration.GetDefaultConfiguration();
            return new GetConfigurationQueryResult(configuration);
        }
    }
}
