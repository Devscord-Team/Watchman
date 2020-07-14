using System;
using System.Collections.Generic;
using System.Text;
using Watchman.DomainModel.Messages;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Users
{
    public class WarnEvent : Entity
    {
        public User Receiver { get; private set; }
        public User Grantor { get; private set; }
        public string Reason { get; private set; }
        public DateTime GaveAt { get; private set; }
        public ulong ServerId { get; private set; }

        public WarnEvent(User grantor, User receiver, string reason, DateTime gaveAt, ulong serverId)
        {
            this.Grantor = grantor;
            this.Receiver = receiver;
            this.Reason = reason;
            this.GaveAt = gaveAt;
            this.ServerId = serverId;
        }

        public string ToString(bool serverId =false)
        {
            return
                "Date: " + GaveAt.ToString() +
                "\nGranted by: " + Grantor?.Name +
                "\nReceiver: " + Receiver?.Name +
                "\nReason: " + Reason +
                (serverId ? "\nServer id: " + ServerId.ToString() : "");
        }
    }
}
