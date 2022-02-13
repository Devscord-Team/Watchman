using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Protection.Mutes.Commands;
using Watchman.DomainModel.Protection.Mutes.Queries;

namespace Watchman.DomainModel.Protection.Mutes.Services
{
    public interface IMutingService
    {
        Task<bool> ShouldUserBeMuted(MuteEvent muteEvent);
        IEnumerable<MuteEvent> GetNotUnmutedMuteEvents(ulong serverId);
        MuteEvent GetNotUnmutedUserMuteEvent(ulong serverId, ulong userId);
        Task MarkAsUnmuted(MuteEvent muteEvent);
    }

    public class MutingService
    {
        private readonly ICommandBus commandBus;
        private readonly IQueryBus queryBus;

        public MutingService(ICommandBus commandBus, IQueryBus queryBus)
        {
            this.commandBus = commandBus;
            this.queryBus = queryBus;
        }

        public async Task<bool> ShouldUserBeMuted(MuteEvent muteEvent)
        {
            var possiblePreviousUserMuteEvent = this.GetNotUnmutedUserMuteEvent(muteEvent.ServerId, muteEvent.UserId);
            var shouldJustMuteAgainTheSameMuteEvent = possiblePreviousUserMuteEvent?.Id == muteEvent.Id;

            if (possiblePreviousUserMuteEvent != null && !shouldJustMuteAgainTheSameMuteEvent)
            {
                await this.MarkAsUnmuted(possiblePreviousUserMuteEvent);
                return false;
            }
            return true;
        }

        public IEnumerable<MuteEvent> GetNotUnmutedMuteEvents(ulong serverId)
            => this.queryBus.Execute(new GetMuteEventsQuery(serverId, takeOnlyNotUnmuted: true))
            .MuteEvents;


        // in the same time there should exists only one MUTED MuteEvent per user per server (FirstOrDefault)
        public MuteEvent GetNotUnmutedUserMuteEvent(ulong serverId, ulong userId)
            => this.queryBus.Execute(new GetMuteEventsQuery(serverId, takeOnlyNotUnmuted: true, userId))
            .MuteEvents.FirstOrDefault();

        public Task MarkAsUnmuted(MuteEvent muteEvent)
            => this.commandBus.ExecuteAsync(new MarkMuteEventAsUnmutedCommand(muteEvent.Id));
    }
}
