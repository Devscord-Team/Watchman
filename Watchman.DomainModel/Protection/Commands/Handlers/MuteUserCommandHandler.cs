using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Protection.Commands.Handlers
{
    public class MuteUserCommandHandler : ICommandHandler<MuteUserCommand>
    {
        public Task HandleAsync(MuteUserCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
