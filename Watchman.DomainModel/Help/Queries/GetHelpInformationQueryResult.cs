using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Help.Queries
{
    public class GetHelpInformationQueryResult : IQueryResult
    {
        public IEnumerable<DefaultHelpInformation> HelpInformations { get; }

        public GetHelpInformationQueryResult(IEnumerable<DefaultHelpInformation> helpInformations)
        {
            this.HelpInformations = helpInformations;
        }
    }
}
