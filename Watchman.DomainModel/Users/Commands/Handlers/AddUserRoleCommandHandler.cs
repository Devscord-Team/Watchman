using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Users.Commands.Handlers
{
    public class AddUserRoleCommandHandler : ICommandHandler<AddUserRoleCommand>
    {
        public Task HandleAsync(AddUserRoleCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
