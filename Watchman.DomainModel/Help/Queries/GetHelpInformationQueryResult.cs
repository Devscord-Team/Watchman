using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Help.Queries
{
    public class GetHelpInformationQueryResult : IQueryResult
    {
        public IEnumerable<ServerHelpInformation> HelpInformations { get; }

        public GetHelpInformationQueryResult(IEnumerable<ServerHelpInformation> helpInformations)
        {
            this.HelpInformations = helpInformations;
        }
    }
}
