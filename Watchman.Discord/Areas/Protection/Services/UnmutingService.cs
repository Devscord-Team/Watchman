using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Serilog;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Protection.Services.Commands;
using Watchman.DomainModel.Protection.Mutes;
using Watchman.DomainModel.Protection.Mutes.Services;

namespace Watchman.Discord.Areas.Protection.Services
{
    public interface IUnmutingService : ICyclicService
    {
        void UnmuteInFuture(Contexts contexts, MuteEvent muteEvent, UserContext userToUnmute);
    }

    public class UnmutingService : IUnmutingService
    {
        private const int SHORT_MUTE_TIME_IN_MINUTES = 15;
        private readonly ICommandBus commandBus;
        private readonly IMutingService mutingService;
        private readonly IUsersService _usersService;
        private readonly IDiscordServersService _discordServersService;
        private readonly HashSet<Guid> _muteEventsAlreadyBeingHandled = new HashSet<Guid>();

        public UnmutingService(ICommandBus commandBus, IMutingService mutingService, IUsersService usersService, IDiscordServersService discordServersService)
        {
            this.commandBus = commandBus;
            this.mutingService = mutingService;
            this._usersService = usersService;
            this._discordServersService = discordServersService;
        }

        public async Task Refresh()
        {
            await foreach (var server in this._discordServersService.GetDiscordServersAsync())
            {
                var serverMuteEvents = this.mutingService.GetNotUnmutedMuteEvents(server.Id).ToList();
                if (!serverMuteEvents.Any())
                {
                    continue;
                }
                var contexts = new Contexts();
                contexts.SetContext(server);
                var textChannels = server.GetTextChannels().ToList();
                foreach (var muteEvent in serverMuteEvents.Where(this.ShouldBeConsideredAsShortMute))
                {
                    var user = await this._usersService.GetUserByIdAsync(server, muteEvent.UserId);
                    var channel = textChannels.FirstOrDefault(x => x.Id == muteEvent.MutedOnChannelId);
                    if (user == null)
                    {
                        await this.mutingService.MarkAsUnmuted(muteEvent);
                        continue;
                    }
                    contexts.SetContext(user);
                    if (channel != null)
                    {
                        contexts.SetContext(channel);
                    }
                    this.UnmuteInShortTime(contexts, muteEvent, user);
                }
            }
        }

        public void UnmuteInFuture(Contexts contexts, MuteEvent muteEvent, UserContext userToUnmute)
        {
            if (this.ShouldBeConsideredAsShortMute(muteEvent))
            {
                this.UnmuteInShortTime(contexts, muteEvent, userToUnmute);
                return;
            }
        }

        private bool ShouldBeConsideredAsShortMute(MuteEvent muteEvent)
        {
            return muteEvent.TimeRange.End < DateTime.UtcNow.AddMinutes(SHORT_MUTE_TIME_IN_MINUTES);
        }

        private async void UnmuteInShortTime(Contexts contexts, MuteEvent muteEvent, UserContext userToUnmute)
        {
            if (this._muteEventsAlreadyBeingHandled.Contains(muteEvent.Id))
            {
                return;
            }
            if (muteEvent.UserId != userToUnmute.Id)
            {
                throw new ArgumentException($"value of {nameof(muteEvent.UserId)} is different than {nameof(userToUnmute.Id)}");
            }
            if (muteEvent.TimeRange.End > DateTime.UtcNow)
            {
                this._muteEventsAlreadyBeingHandled.Add(muteEvent.Id);
                await Task.Delay(muteEvent.TimeRange.End - DateTime.UtcNow);
            }
            await this.commandBus.ExecuteAsync(new UnmuteSpecificEventCommand(contexts, muteEvent, userToUnmute));
            this._muteEventsAlreadyBeingHandled.Remove(muteEvent.Id);
        }
    }
}
