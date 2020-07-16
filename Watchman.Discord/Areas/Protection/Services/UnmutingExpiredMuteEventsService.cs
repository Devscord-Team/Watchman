using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Users.Queries;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class UnmutingExpiredMuteEventsService
    {
        private readonly IQueryBus _queryBus;
        private readonly UsersService _usersService;
        private readonly UsersRolesService _usersRolesService;
        private readonly UnmutingService _unmutingService;
        private readonly MutingHelperService _mutingHelperService;

        public UnmutingExpiredMuteEventsService(IQueryBus queryBus, UsersService usersService, UsersRolesService usersRolesService, UnmutingService unmutingService, MutingHelperService mutingHelperService)
        {
            this._queryBus = queryBus;
            this._usersService = usersService;
            this._usersRolesService = usersRolesService;
            this._unmutingService = unmutingService;
            this._mutingHelperService = mutingHelperService;
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
                .Where(x => x.IsUnmuted == false)
                .ToList();

            var contexts = new Contexts();
            contexts.SetContext(server);
            foreach (var muteEvent in notUnmutedEvents)
            {
                var user = this._usersService.GetUserById(server, muteEvent.UserId);
                var channel = server.TextChannels.FirstOrDefault(x => x.Id == muteEvent.MutedOnChannelId);
                if (user == null)
                {
                    await this._mutingHelperService.MarkAsUnmuted(muteEvent);
                    continue;
                }
                contexts.SetContext(user);
                if (channel != null)
                {
                    contexts.SetContext(channel);
                }
                this._unmutingService.UnmuteInFuture(contexts, muteEvent, user);
            }
        }
    }
}
