using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Help.Queries
{
    public class GetHelpInformationQueryResult : IQueryResult
    {
        public IEnumerable<HelpInformation> HelpInformations { get; }

        public GetHelpInformationQueryResult(IEnumerable<HelpInformation> helpInformations) => this.HelpInformations = helpInformations;
    }
}
