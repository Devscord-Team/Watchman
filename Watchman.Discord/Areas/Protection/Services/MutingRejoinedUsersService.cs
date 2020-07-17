using Devscord.DiscordFramework.Middlewares.Contexts;
using System.Threading.Tasks;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class MutingRejoinedUsersService
    {
        private readonly MutingService _mutingService;
        private readonly MutingHelper _mutingHelper;

        public MutingRejoinedUsersService(MutingService mutingService, MutingHelper mutingHelper)
        {
            this._mutingService = mutingService;
            this._mutingHelper = mutingHelper;
        }

        public async Task MuteAgainIfNeeded(Contexts contexts)
        {
            var notUnmuted = this._mutingHelper.GetNotUnmutedUserMuteEvent(contexts.Server.Id, contexts.User.Id);
            if (notUnmuted != null)
            {
                await this._mutingService.MuteUserOrOverwrite(contexts, notUnmuted, contexts.User);
            }
        }
    }
}
