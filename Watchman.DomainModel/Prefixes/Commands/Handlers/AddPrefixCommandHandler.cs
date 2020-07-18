using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Prefixes.Commands.Handlers
{
    public class AddPrefixCommandHandler : ICommandHandler<AddPrefixCommand>
    {
        public Task HandleAsync(AddPrefixCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
