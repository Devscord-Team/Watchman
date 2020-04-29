using System;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Settings.Commands
{
    public class AddInitEventCommand : ICommand
    {
        public ulong ServerId { get; set; }
        public DateTime EndedAt { get; set; }

        public AddInitEventCommand(ulong serverId, DateTime endedAt)
        {
            ServerId = serverId;
            EndedAt = endedAt;
        }
    }
}
