using System;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Protection.Commands
{
    public class MarkMuteEventAsUnmutedCommand : ICommand
    {
        public Guid MuteEventGuid { get; }

        public MarkMuteEventAsUnmutedCommand(Guid muteEventGuid)
        {
            this.MuteEventGuid = muteEventGuid;
        }
    }
}
