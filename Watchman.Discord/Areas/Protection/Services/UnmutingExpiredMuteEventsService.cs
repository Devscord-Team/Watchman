using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Watchman.Cqrs;
using Watchman.DomainModel.Users;
using Watchman.DomainModel.Users.Commands;
using Watchman.DomainModel.Users.Queries;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class UnmutingExpiredMuteEventsService
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly UsersService _usersService;
        private readonly UsersRolesService _usersRolesService;

        public UnmutingExpiredMuteEventsService(IQueryBus queryBus, ICommandBus commandBus, UsersService usersService, UsersRolesService usersRolesService)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _usersService = usersService;
            _usersRolesService = usersRolesService;
        }

        public void UnmuteUsersInit(DiscordServerContext server)
        {
            var users = _usersService.GetUsers(server).ToList();
            var muteRole = _usersRolesService.GetRoleByName(UsersRolesService.MUTED_ROLE_NAME, server);

            if (!users.Any() || muteRole == null)
            {
                return;
            }

            var query = new GetMuteEventsQuery(server.Id);
            var notUnmutedEvents = _queryBus.Execute(query).MuteEvents
                .Where(x => x.Unmuted == false)
                .ToList();

            foreach (var muteEvent in notUnmutedEvents)
            {
                if (muteEvent.TimeRange.End < DateTime.UtcNow)
                {
                    RemoveMuteRole(muteEvent, server, users, muteRole);
                }
                else
                {
                    RemoveInFuture(muteEvent, server, users, muteRole);
                }
            }
        }

        private async void RemoveMuteRole(MuteEvent muteEvent, DiscordServerContext server, IEnumerable<UserContext> users, UserRole muteRole)
        {
            var user = users.FirstOrDefault(userContext => userContext.Id == muteEvent.UserId);
            if (user == null)
            {
                return;
            }

            await _usersService.RemoveRole(muteRole, user, server);
            var command = new MarkMuteEventAsUnmutedCommand(muteEvent.Id);
            await _commandBus.ExecuteAsync(command);
        }

        private async void RemoveInFuture(MuteEvent muteEvent, DiscordServerContext server, IEnumerable<UserContext> users, UserRole muteRole)
        {
            await Task.Delay(muteEvent.TimeRange.End - DateTime.UtcNow);
            RemoveMuteRole(muteEvent, server, users, muteRole);
        }
    }
}
