using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Prefixes.Commands
{
    public class AddPrefixCommand : ICommand
    {
        public ulong ServerId { get; }
        public string Value { get; }

        public AddPrefixCommand(ulong serverId, string value)
        {
            this.ServerId = serverId;
            this.Value = value;
        }
    }
}
