using System;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Configuration.Commands
{
    public class AddInitEventCommand : ICommand
    {
        public ulong ServerId { get; private set; }
        public DateTime EndedAt { get; private set; }

        public AddInitEventCommand(ulong serverId, DateTime endedAt)
        {
            this.ServerId = serverId;
            this.EndedAt = endedAt;
        }
    }
}
