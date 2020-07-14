using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Users.Queries
{
    public class GetWarnEventsQueryResults : IQueryResult
    {
        public IEnumerable<WarnEvent> WarnEvents { get; }

        public GetWarnEventsQueryResults(IEnumerable<WarnEvent> warnEvents)
        {
            this.WarnEvents = warnEvents;
        }
    }
}
