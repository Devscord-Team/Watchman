using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Configuration.Queries.Handlers
{
    public class GetConfigurationItemsQueryHandler : IQueryHandler<GetConfigurationItemsQuery, GetConfigurationItemsQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetConfigurationItemsQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetConfigurationItemsQueryResult Handle(GetConfigurationItemsQuery query)
        {
            using var session = this._sessionFactory.CreateMongo();
            var configurationItems = session.Get<ConfigurationItem>();
            if(query.Group != null)
            {
                configurationItems = configurationItems.Where(x => x.Group == query.Group);
            }
            return new GetConfigurationItemsQueryResult(configurationItems);
        }
    }
}
