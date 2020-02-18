#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Watchman.Cqrs;
using Watchman.DomainModel.Users;
using Watchman.DomainModel.Users.Commands;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class MuteService
    {
        private readonly ICommandBus _commandBus;
        private readonly UsersService _usersService;
        private readonly UsersRolesService _usersRolesService;

        public MuteService(ICommandBus commandBus, UsersService usersService, UsersRolesService usersRolesService)
        {
            this._commandBus = commandBus;
            this._usersService = usersService;
            this._usersRolesService = usersRolesService;
        }

        public async Task MuteUser(UserContext userToMute, DiscordServerContext serverContext)
        {
            var muteRole = GetMuteRole(serverContext);
            await AssignMuteRoleAsync(muteRole, userToMute, serverContext);
        }

        public async Task UnmuteIfNeeded(DiscordServerContext server, UserContext userToUnmute, IEnumerable<MuteEvent> userMuteEvents)
        {
            var eventToUnmute = GetNotUnmutedEvent(userMuteEvents);
            if (eventToUnmute == null)
            {
                return;
            }
            await UnmuteUser(userToUnmute, eventToUnmute, server);
        }

        private UserRole GetMuteRole(DiscordServerContext server)
        {
            var muteRole = _usersRolesService.GetRoleByName(UsersRolesService.MUTED_ROLE_NAME, server);

            if (muteRole == null)
            {
                throw new RoleNotFoundException(UsersRolesService.MUTED_ROLE_NAME);
            }
            return muteRole;
        }

        private async Task AssignMuteRoleAsync(UserRole muteRole, UserContext userToMute, DiscordServerContext server)
        {
            await _usersService.AddRole(muteRole, userToMute, server);
        }
        private MuteEvent? GetNotUnmutedEvent(IEnumerable<MuteEvent> userMuteEvents)
        {
            return userMuteEvents.FirstOrDefault(x => x.Unmuted == false);
        }

        private async Task UnmuteUser(UserContext mutedUser, MuteEvent muteEvent, DiscordServerContext serverContext)
        {
            var muteRole = GetMuteRole(serverContext);
            await RemoveMuteRoleAsync(muteRole, mutedUser, serverContext);
            await MarkAsUnmuted(muteEvent);
        }

        private async Task RemoveMuteRoleAsync(UserRole muteRole, UserContext userToUnmute, DiscordServerContext server)
        {
            await _usersService.RemoveRole(muteRole, userToUnmute, server);
        }

        private async Task MarkAsUnmuted(MuteEvent muteEvent)
        {
            var command = new MarkMuteEventAsUnmutedCommand(muteEvent.Id);
            await _commandBus.ExecuteAsync(command);
        }
    }
}
