using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Mute.Queries
{
    public class GetMuteEventsFromBaseQueryResult : IQueryResult
    {
        public IEnumerable<MuteEvent> MuteEvents { get; }

        public GetMuteEventsFromBaseQueryResult(IEnumerable<MuteEvent> muteEvents)
        {
            MuteEvents = muteEvents;
        }
    }
}
