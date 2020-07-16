using System;
using System.Collections.Generic;
using System.Text;
using Watchman.DomainModel.Messages;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Warnings
{
    public class WarnEvent : Entity
    {
        public ulong GrantorId { get; private set; }
        public ulong ReceiverId { get; private set; }
        public string Reason { get; private set; }
        public ulong ServerId { get; private set; }

        public WarnEvent(ulong grantorId, ulong receiverId, string reason, ulong serverId)
        {
            this.GrantorId = grantorId;
            this.ReceiverId = receiverId;
            this.Reason = reason;
            this.ServerId = serverId;
            this.CreatedAt = DateTime.Now;
        }
    }
}
