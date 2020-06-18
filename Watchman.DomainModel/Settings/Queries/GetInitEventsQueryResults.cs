using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Settings.Queries
{
    public class GetInitEventsQueryResults : IQueryResult
    {
        public IEnumerable<InitEvent> InitEvents { get; private set; }

        public GetInitEventsQueryResults(IEnumerable<InitEvent> initEvents) => this.InitEvents = initEvents;
    }
}
