using System.Collections.Generic;
using Watchman.Cqrs;
using Watchman.DomainModel.Muting;

namespace Watchman.DomainModel.Muting.Queries
{
    public class GetMuteEventsQueryResult : IQueryResult
    {
        public IEnumerable<MuteEvent> MuteEvents { get; }

        public GetMuteEventsQueryResult(IEnumerable<MuteEvent> muteEvents)
        {
            this.MuteEvents = muteEvents;
        }
    }
}
