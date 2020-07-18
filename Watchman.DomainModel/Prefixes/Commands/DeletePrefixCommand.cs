using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Prefixes.Commands
{
    public class DeletePrefixCommand : ICommand
    {
        public Prefix Prefix { get; }

        public DeletePrefixCommand(Prefix prefix)
        {
            this.Prefix = prefix;
        }
    }
}
