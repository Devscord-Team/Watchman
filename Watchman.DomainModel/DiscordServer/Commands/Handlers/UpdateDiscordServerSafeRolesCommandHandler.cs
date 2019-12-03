using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;

namespace Watchman.DomainModel.DiscordServer.Commands.Handlers
{
    public class UpdateDiscordServerSafeRolesCommandHandler : ICommandHandler<UpdateDiscordServerSafeRolesCommand>
    {
        public Task HandleAsync(UpdateDiscordServerSafeRolesCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
