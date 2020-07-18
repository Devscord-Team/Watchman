using System;
using System.Threading.Tasks;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Mutes.Commands.Handlers
{
    public class AddUserRoleCommandHandler : ICommandHandler<AddUserRoleCommand>
    {
        public AddUserRoleCommandHandler()
        {

        }

        public Task HandleAsync(AddUserRoleCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
