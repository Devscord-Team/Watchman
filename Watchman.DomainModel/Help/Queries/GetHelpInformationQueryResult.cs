using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Help.Queries
{
    public class GetHelpInformationQueryResult : IQueryResult
    {
        public List<ServerHelpInformation> HelpInformations { get; }

        public GetHelpInformationQueryResult(List<ServerHelpInformation> helpInformations)
        {
            this.HelpInformations = helpInformations;
        }
    }
}
