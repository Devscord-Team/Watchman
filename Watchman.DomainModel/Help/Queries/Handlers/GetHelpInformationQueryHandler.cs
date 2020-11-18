using System.Linq;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Help.Queries.Handlers
{
    public class GetHelpInformationQueryHandler : IQueryHandler<GetHelpInformationQuery, GetHelpInformationQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetHelpInformationQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetHelpInformationQueryResult Handle(GetHelpInformationQuery query)
        {
            var session = this._sessionFactory.Create();
            var allHelpInfos = session.Get<HelpInformation>().ToList();
            var defaultHelpInfos = allHelpInfos.Where(x => x.IsDefault);
            var customHelpInfos = allHelpInfos.Where(x => x.ServerId == query.ServerId).ToList();

            customHelpInfos.AddRange(defaultHelpInfos.Where(x => customHelpInfos.All(c => c.CommandName != x.CommandName)));
            return new GetHelpInformationQueryResult(customHelpInfos);
        }
    }
}
