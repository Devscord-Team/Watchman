using Devscord.EventStore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.DomainModel.ServerPrefixes.Events
{
    public class PrefixAddedToServerEvent : Event
    {
        public ulong ServerId { get; set; }
        public string Prefix { get; set; }

        public PrefixAddedToServerEvent()
        {
        }

        public PrefixAddedToServerEvent(ulong serverId, string prefix)
        {
            this.ServerId = serverId;
            this.Prefix = prefix;
        }
    }
}
