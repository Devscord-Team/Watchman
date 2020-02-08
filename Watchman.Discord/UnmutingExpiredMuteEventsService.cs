using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Watchman.Cqrs;
using Watchman.DomainModel.Mute.Queries;

namespace Watchman.Discord
{
    public class UnmutingExpiredMuteEventsService
    {
        private readonly IQueryBus _queryBus;
        private readonly UsersService _usersService;
        private readonly UsersRolesService _usersRolesService;

        public UnmutingExpiredMuteEventsService(IQueryBus queryBus, UsersService usersService, UsersRolesService usersRolesService)
        {
            _queryBus = queryBus;
            _usersService = usersService;
            _usersRolesService = usersRolesService;
        }

        public void UnmuteUsersInit(DiscordServerContext server)
        {
            var query = new GetMuteEventsFromBaseQuery(server.Id);
            var muteEvents = _queryBus.Execute(query).MuteEvents;

            var shouldBeUnmuted = muteEvents.Where(x => x.Unmuted == false && x.TimeRange.End < DateTime.UtcNow);
            RemoveMuteRole(shouldBeUnmuted.Select(x => x.UserId), server);
        }

        private async void RemoveMuteRole(IEnumerable<ulong> usersIds, DiscordServerContext server)
        {
            var users = _usersService.GetUsers(server).ToList();
            var muteRole = _usersRolesService.GetRoleByName(UsersRolesService.MUTED_ROLE_NAME, server);

            if (!users.Any() || muteRole == null)
            {
                return;
            }

            foreach (var uId in usersIds.ToList())
            {
                var user = users.FirstOrDefault(userContext => userContext.Id == uId);
                if (user == null)
                {
                    return;
                }

                await _usersService.RemoveRole(muteRole, user, server);
            }
        }
    }
}
