using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Help.Queries
{
    public class GetHelpInformationQueryResult : IQueryResult
    {
        public List<HelpInformation> HelpInformations { get; }

        public GetHelpInformationQueryResult(List<HelpInformation> helpInformations)
        {
            this.HelpInformations = helpInformations;
        }
    }
}
