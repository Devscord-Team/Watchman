using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.MemoryStorage.Entities;
using Microsoft.AspNetCore.Mvc;
using Watchman.Cqrs;
using Watchman.DomainModel.DiscordServer.Commands;

namespace Watchman.Web.Areas.Administration.Controllers
{
    public class SafeRolesController : BaseApiController
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public SafeRolesController(IQueryBus queryBus, ICommandBus commandBus)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
        }

        [HttpPost]
        public async Task SetSafeRole(string roleName, ulong serverId = 0)
        {
            var command = new SetRoleAsSafeCommand(roleName,serverId);
            await this._commandBus.ExecuteAsync(command);
        }

        [HttpPost]
        public async Task RemoveSafeRole(string roleName, ulong serverId = 0)
        {
            var command = new SetRoleAsUnsafeCommand(roleName, serverId);
            await this._commandBus.ExecuteAsync(command);
        }
    }
}
