using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Help.Queries
{
    public class GetHelpInformationsQueryResult : IQueryResult
    {
        public IEnumerable<HelpInformation> HelpInformations { get; }

        public GetHelpInformationsQueryResult(IEnumerable<HelpInformation> helpInformations)
        {
            this.HelpInformations = helpInformations;
        }
    }
}
