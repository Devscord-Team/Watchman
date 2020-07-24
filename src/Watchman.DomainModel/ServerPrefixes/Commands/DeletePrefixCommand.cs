using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.ServerPrefixes.Commands
{
    public class DeletePrefixCommand : ICommand
    {
        public ulong ServerId { get; }
        public string Prefix { get; }

        public DeletePrefixCommand(ulong serverId, string prefix)
        {
            this.ServerId = serverId;
            this.Prefix = prefix;
        }
    }
}
