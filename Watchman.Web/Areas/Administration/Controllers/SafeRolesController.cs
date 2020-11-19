using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Watchman.Cqrs;
using Watchman.DomainModel.DiscordServer.Commands;

namespace Watchman.Web.Areas.Administration.Controllers
{
    public class SafeRolesController : BaseApiController
    {
        private readonly ICommandBus _commandBus;

        public SafeRolesController(ICommandBus commandBus)
        {
            this._commandBus = commandBus;
        }

        [HttpPost]
        public Task SetSafeRole(ulong roleId, ulong serverId)
        {
            var command = new SetRoleAsSafeCommand(roleId, serverId);
            return this._commandBus.ExecuteAsync(command);
        }

        [HttpPost]
        public Task RemoveSafeRole(ulong roleId, ulong serverId)
        {
            var command = new SetRoleAsUnsafeCommand(roleId, serverId);
            return this._commandBus.ExecuteAsync(command);
        }
    }
}