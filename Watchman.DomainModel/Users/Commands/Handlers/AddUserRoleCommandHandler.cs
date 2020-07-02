using System;
using System.Threading.Tasks;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Users.Commands.Handlers
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
