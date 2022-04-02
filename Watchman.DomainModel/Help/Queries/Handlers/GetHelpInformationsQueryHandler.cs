using System.Linq;
using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Help.Queries.Handlers
{
    public class GetHelpInformationsQueryHandler : IQueryHandler<GetHelpInformationsQuery, GetHelpInformationsQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetHelpInformationsQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetHelpInformationsQueryResult Handle(GetHelpInformationsQuery query)
        {
            var session = this._sessionFactory.CreateMongo();
            var allHelpInfos = session.Get<HelpInformation>().ToList();
            var defaultHelpInfos = allHelpInfos.Where(x => x.IsDefault);
            var customHelpInfos = allHelpInfos.Where(x => x.ServerId == query.ServerId).ToList();

            customHelpInfos.AddRange(defaultHelpInfos.Where(x => customHelpInfos.All(c => c.CommandName != x.CommandName)));
            return new GetHelpInformationsQueryResult(customHelpInfos);
        }
    }
}
