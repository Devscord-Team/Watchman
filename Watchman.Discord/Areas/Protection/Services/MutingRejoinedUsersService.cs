using Devscord.DiscordFramework.Middlewares.Contexts;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Users;
using Watchman.DomainModel.Users.Queries;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class MutingRejoinedUsersService
    {
        private readonly IQueryBus _queryBus;
        private readonly MutingService _mutingService;

        public MutingRejoinedUsersService(IQueryBus queryBus, MutingService mutingService)
        {
            this._queryBus = queryBus;
            this._mutingService = mutingService;
        }

        public async Task MuteAgainIfNeeded(Contexts contexts)
        {
            var notUnmuted = this.GetNotUnmutedMuteEvent(contexts.Server, contexts.User);
            if (notUnmuted != null)
            {
                await this._mutingService.MuteUserOrOverwrite(contexts, notUnmuted, contexts.User);
            }
        }

        public MuteEvent GetNotUnmutedMuteEvent(DiscordServerContext server, UserContext userContext)
        {
            var getMuteEvents = new GetMuteEventsQuery(server.Id);
            var allServerMuteEvents = this._queryBus.Execute(getMuteEvents).MuteEvents;
            var userMuteEvents = allServerMuteEvents.Where(x => x.UserId == userContext.Id);

            return userMuteEvents.FirstOrDefault(x => !x.IsUnmuted);
        }
    }
}
