using System;
using System.Threading.Tasks;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Mutes.Commands.Handlers
{
    public class RemoveUserRoleCommandHandler : ICommandHandler<RemoveUserRoleCommand>
    {
        public Task HandleAsync(RemoveUserRoleCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
