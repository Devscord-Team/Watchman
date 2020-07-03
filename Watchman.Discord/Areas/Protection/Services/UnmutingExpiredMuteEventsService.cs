using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._usersService = usersService;
            this._usersRolesService = usersRolesService;
        }

        public void UnmuteUsersInit(DiscordServerContext server)
        {
            var users = this._usersService.GetUsers(server).ToList();
            var muteRole = this._usersRolesService.GetRoleByName(UsersRolesService.MUTED_ROLE_NAME, server);

            if (!users.Any() || muteRole == null)
            {
                return;
            }

            var query = new GetMuteEventsQuery(server.Id);
            var notUnmutedEvents = this._queryBus.Execute(query).MuteEvents
                .Where(x => x.Unmuted == false)
                .ToList();

            foreach (var muteEvent in notUnmutedEvents)
            {
                if (muteEvent.TimeRange.End < DateTime.UtcNow)
                {
                    this.RemoveMuteRole(muteEvent, server, users, muteRole);
                }
                else
                {
                    this.RemoveInFuture(muteEvent, server, users, muteRole);
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

            await this._usersService.RemoveRole(muteRole, user, server);
            var command = new MarkMuteEventAsUnmutedCommand(muteEvent.Id);
            await this._commandBus.ExecuteAsync(command);
        }

        private async void RemoveInFuture(MuteEvent muteEvent, DiscordServerContext server, IEnumerable<UserContext> users, UserRole muteRole)
        {
            await Task.Delay(muteEvent.TimeRange.End - DateTime.UtcNow);
            this.RemoveMuteRole(muteEvent, server, users, muteRole);
        }
    }
}
