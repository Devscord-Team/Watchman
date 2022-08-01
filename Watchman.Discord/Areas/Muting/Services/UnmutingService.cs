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
using Watchman.Discord.Areas.Muting.Services.Commands;
using Watchman.DomainModel.Muting;
using Watchman.DomainModel.Muting.Services;

namespace Watchman.Discord.Areas.Muting.Services
{
    public interface IUnmutingService
    {
        void UnmuteInFuture(Contexts contexts, MuteEvent muteEvent, UserContext userToUnmute);
        bool ShouldBeConsideredAsShortMute(MuteEvent muteEvent);
    }

    public class UnmutingService : IUnmutingService
    {
        private const int SHORT_MUTE_TIME_IN_MINUTES = 15;
        private readonly ICommandBus _commandBus;
        private readonly HashSet<Guid> _muteEventsAlreadyBeingHandled = new();

        public UnmutingService(ICommandBus commandBus)
        {
            this._commandBus = commandBus;
        }

        public void UnmuteInFuture(Contexts contexts, MuteEvent muteEvent, UserContext userToUnmute)
        {
            if (this.ShouldBeConsideredAsShortMute(muteEvent))
            {
                this.UnmuteInShortTime(contexts, muteEvent, userToUnmute);
            }
        }

        public bool ShouldBeConsideredAsShortMute(MuteEvent muteEvent)
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
            await this._commandBus.ExecuteAsync(new UnmuteSpecificEventCommand(contexts, muteEvent, userToUnmute));
            this._muteEventsAlreadyBeingHandled.Remove(muteEvent.Id);
        }
    }
}
