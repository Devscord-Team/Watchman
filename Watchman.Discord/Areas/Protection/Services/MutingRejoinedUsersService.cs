using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Watchman.Cqrs;
using Watchman.DomainModel.Users;
using Watchman.DomainModel.Users.Queries;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class MutingRejoinedUsersService
    {
        private readonly IQueryBus _queryBus;
        private readonly MuteService _muteService;

        public MutingRejoinedUsersService(IQueryBus queryBus, MuteService muteService)
        {
            _queryBus = queryBus;
            _muteService = muteService;
        }

        public async Task MuteAgainIfNeeded(Contexts contexts)
        {
            var notUnmuted = GetNotUnmutedMuteEvent(contexts.Server, contexts.User);
            if (notUnmuted != null)
            {
                await _muteService.MuteUserOrOverwrite(contexts, notUnmuted, contexts.User);
            }
        }

        public MuteEvent GetNotUnmutedMuteEvent(DiscordServerContext server, UserContext userContext)
        {
            var getMuteEvents = new GetMuteEventsQuery(server.Id);
            var allServerMuteEvents = _queryBus.Execute(getMuteEvents).MuteEvents;
            var userMuteEvents = allServerMuteEvents.Where(x => x.UserId == userContext.Id);

            return userMuteEvents.FirstOrDefault(x => x.Unmuted == false);
        }
    }
}
