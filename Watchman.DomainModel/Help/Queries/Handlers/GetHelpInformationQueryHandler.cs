using System.Collections.Generic;
using System.IO;
using System.Linq;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Help.Queries.Handlers
{
    public class GetHelpInformationQueryHandler : IQueryHandler<GetHelpInformationQuery, GetHelpInformationQueryResult>
    {
        private const string _helpFileName = "helpInformation.json";

        public GetHelpInformationQueryResult Handle(GetHelpInformationQuery query)
        {
            var helpInformation = GetHelpInformationFromFile(query.ServerId);

            if ( !helpInformation.Any())
            {
                helpInformation = GetHelpInformationFromDb(query.Session, query.ServerId);
            }

            return new GetHelpInformationQueryResult(helpInformation);
        }

        private IEnumerable<ServerHelpInformation> GetHelpInformationFromDb(ISession session, ulong serverId)
        {
            return session.Get<ServerHelpInformation>();
        }

        private IEnumerable<ServerHelpInformation> GetHelpInformationFromFile(ulong serverId)
        {
            if ( !File.Exists(_helpFileName))
                return new List<ServerHelpInformation>();
            
            var allText = File.ReadAllText(_helpFileName);
            var helps = new List<ServerHelpInformation>();

            // todo: parse json file to list

            return helps;
        }
    }
}
