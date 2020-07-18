using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Prefixes.Commands.Handlers
{
    public class RemovePrefixCommandHandler : ICommandHandler<RemovePrefixCommand>
    {
        public Task HandleAsync(RemovePrefixCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
