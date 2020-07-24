using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Configuration.Queries
{
    public class GetInitEventsQueryResults : IQueryResult
    {
        public IEnumerable<InitEvent> InitEvents { get; private set; }

        public GetInitEventsQueryResults(IEnumerable<InitEvent> initEvents)
        {
            this.InitEvents = initEvents;
        }
    }
}
