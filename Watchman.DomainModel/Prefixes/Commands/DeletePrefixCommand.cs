using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.ServerPrefixes.Commands
{
    public class DeletePrefixCommand : ICommand
    {
        public ServerPrefixes Prefix { get; }

        public DeletePrefixCommand(ServerPrefixes prefix)
        {
            this.Prefix = prefix;
        }
    }
}
