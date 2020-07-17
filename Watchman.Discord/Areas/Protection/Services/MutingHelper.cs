using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Watchman.Cqrs;
using Watchman.DomainModel.Users;
using Watchman.DomainModel.Users.Commands;
using Watchman.DomainModel.Users.Queries;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class MutingHelper
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly UsersRolesService _usersRolesService;

        public MutingHelper(IQueryBus queryBus, ICommandBus commandBus, UsersRolesService usersRolesService)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._usersRolesService = usersRolesService;
        }

        public UserRole GetMuteRole(DiscordServerContext server)
        {
            var muteRole = this._usersRolesService.GetRoleByName(UsersRolesService.MUTED_ROLE_NAME, server);
            if (muteRole == null)
            {
                throw new RoleNotFoundException(UsersRolesService.MUTED_ROLE_NAME);
            }
            return muteRole;
        }

        public IEnumerable<MuteEvent> GetServerNotUnmutedMuteEvents(ulong serverId)
        {
            var query = new GetMuteEventsQuery(serverId);
            return this._queryBus.Execute(query).MuteEvents.Where(x => !x.IsUnmuted);
        }

        public async Task MarkAsUnmuted(MuteEvent muteEvent)
        {
            var command = new MarkMuteEventAsUnmutedCommand(muteEvent.Id);
            await this._commandBus.ExecuteAsync(command);
        }
    }
}
