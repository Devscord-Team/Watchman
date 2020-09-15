using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Warns.Commands
{
    public class RemoveWarnEventsCommand : ICommand
    {
        public ulong? GrantorId { get; set; }
        public ulong? ReceiverId { get; set; }
        public ulong ServerId { get; set; }
        
        public RemoveWarnEventsCommand(ulong? grantorId, ulong? receiverId, ulong serverId)
        {
            this.GrantorId = grantorId;
            this.ReceiverId = receiverId;
            this.ServerId = serverId;
        }
    }
}
