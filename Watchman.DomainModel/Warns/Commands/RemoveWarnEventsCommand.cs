using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Warns.Commands
{
    public class RemoveWarnEventsCommand : ICommand
    {
        public ulong? GrantorId { get; }
        public ulong? ReceiverId { get; }
        public ulong ServerId { get; }
        public DateTime From { get; }
        
        public RemoveWarnEventsCommand(ulong? grantorId, ulong? receiverId, ulong serverId, DateTime from)
        {
            this.GrantorId = grantorId;
            this.ReceiverId = receiverId;
            this.ServerId = serverId;
            this.From = from;
        }
    }
}
