using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Models;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Discord.Areas.Muting.Services;
using Watchman.DomainModel.Muting.Services;

namespace Watchman.Web.Jobs
{
    public class UnmutingUsersInFutureJob : IHangfireJob
    {
        public RefreshFrequent Frequency => RefreshFrequent.Quarterly; // if RefreshFrequent changed remember to change SHORT_MUTE_TIME_IN_MINUTES in unmutingService!
        public bool RunOnStart => true;

        private readonly IUnmutingService _unmutingService;
        private readonly IMutingService _mutingService;
        private readonly IUsersService _usersService;
        private readonly IDiscordServersService _discordServersService;

        public UnmutingUsersInFutureJob(IUnmutingService unmutingService, IMutingService mutingService, IUsersService usersService, IDiscordServersService discordServersService)
        {
            this._unmutingService = unmutingService;
            this._mutingService = mutingService;
            this._usersService = usersService;
            this._discordServersService = discordServersService;
        }

        public async Task Do()
        {
            await foreach (var server in this._discordServersService.GetDiscordServersAsync())
            {
                var serverMuteEvents = this._mutingService.GetNotUnmutedMuteEvents(server.Id).ToList();
                if (!serverMuteEvents.Any())
                {
                    continue;
                }
                var contexts = new Contexts();
                contexts.SetContext(server);
                var textChannels = server.GetTextChannels().ToList();
                foreach (var muteEvent in serverMuteEvents.Where(this._unmutingService.ShouldBeConsideredAsShortMute))
                {
                    var user = await this._usersService.GetUserByIdAsync(server, muteEvent.UserId);
                    var channel = textChannels.FirstOrDefault(x => x.Id == muteEvent.MutedOnChannelId);
                    if (user == null)
                    {
                        await this._mutingService.MarkAsUnmuted(muteEvent);
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
}
