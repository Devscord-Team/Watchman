using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
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
        private readonly MuteService _muteService;

        public UnmutingExpiredMuteEventsService(IQueryBus queryBus, ICommandBus commandBus, UsersService usersService, UsersRolesService usersRolesService, MuteService muteService)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._usersService = usersService;
            this._usersRolesService = usersRolesService;
            this._muteService = muteService;
        }

        public async Task UnmuteUsersInit(DiscordServerContext server)
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

            var contexts = new Contexts();
            contexts.SetContext(server);
            foreach (var muteEvent in notUnmutedEvents)
            {
                var user = this._usersService.GetUserById(server, muteEvent.UserId);
                var channel = server.TextChannels.FirstOrDefault(x => x.Id == muteEvent.ChannelWhereGivenId);
                if (user == null)
                {
                    await this.MarkAsUnmuted(muteEvent);
                    continue;
                }
                contexts.SetContext(user);
                if (channel == null)
                {
                    this._muteService.UnmuteInFuture(contexts, muteEvent, user, sendMessageAfterUnmute: false);
                    continue;
                }
                contexts.SetContext(channel);

                this._muteService.UnmuteInFuture(contexts, muteEvent, user);
            }
        }

        private async Task MarkAsUnmuted(MuteEvent muteEvent)
        {
            var command = new MarkMuteEventAsUnmutedCommand(muteEvent.Id);
            await this._commandBus.ExecuteAsync(command);
        }
    }
}
