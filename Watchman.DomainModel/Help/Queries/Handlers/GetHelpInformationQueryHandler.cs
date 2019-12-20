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
            var serverHelpInformations = GetHelpInformationFromDb(query.Session, query.ServerId).ToList();
            var defaultHelpInformations = GetDefaultHelpInformation(query.Session);

            var helpInformations = new List<DefaultHelpInformation>();

            foreach (var defaultHelp in defaultHelpInformations)
            {
                // todo: check what is better: SequenceEqual or Equals
                var serverHelp = serverHelpInformations.FirstOrDefault(x => x.MethodNames.SequenceEqual(defaultHelp.MethodNames));
                helpInformations.Add(serverHelp ?? defaultHelp);
            }

            return new GetHelpInformationQueryResult(helpInformations);
        }

        private IEnumerable<ServerHelpInformation> GetHelpInformationFromDb(ISession session, ulong serverId)
        {
            return session.Get<ServerHelpInformation>()
                .Where(x => x.ServerId == serverId);
        }

        private IEnumerable<DefaultHelpInformation> GetDefaultHelpInformation(ISession session)
        {
            return session.Get<DefaultHelpInformation>();
        }

        // todo: na razie zostawiam, może się przydać ten skrawek, jak nie to do usunięcia (usunąć przed ostatecznym mergem do mastera)
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
