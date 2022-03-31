using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Muting.Models;
using Watchman.DomainModel.Muting;
using Watchman.DomainModel.Muting.Services;

namespace Watchman.Discord.Areas.Muting.Services.Commands
{
    public class SendMutedUsersDirectMessageCommandHandler : ICommandHandler<SendMutedUsersDirectMessageCommand>
    {
        private readonly IMutingService mutingService;
        private readonly IMessagesServiceFactory messagesServiceFactory;
        private readonly IDirectMessagesService directMessagesService;

        public SendMutedUsersDirectMessageCommandHandler(IMutingService mutingService, IMessagesServiceFactory messagesServiceFactory, IDirectMessagesService directMessagesService)
        {
            this.mutingService = mutingService;
            this.messagesServiceFactory = messagesServiceFactory;
            this.directMessagesService = directMessagesService;
        }

        public async Task HandleAsync(SendMutedUsersDirectMessageCommand command)
        {
            var notUnmutedMuteEvents = this.mutingService.GetNotUnmutedMuteEvents(command.Contexts.Server.Id);
            var messagesService = this.messagesServiceFactory.Create(command.Contexts);
            var mutedUsersMessageData = this.GetMuteEmbedMessage(notUnmutedMuteEvents.ToList());
            if (!mutedUsersMessageData.Values.Any())
            {
                await messagesService.SendResponse(x => x.ThereAreNoMutedUsers());
                return;
            }
            await this.directMessagesService.TrySendEmbedMessage(command.Contexts.User.Id, mutedUsersMessageData.Title, mutedUsersMessageData.Description, mutedUsersMessageData.Values);
        }
        private MutedUsersMessageData GetMuteEmbedMessage(IReadOnlyList<MuteEvent> notUnmutedMuteEvents)
        {
            var title = "Lista wyciszonych użytkowników";
            var description = "Wyciszeni użytkownicy, powody oraz data wygaśnięcia";
            var values = new Dictionary<string, Dictionary<string, string>>();
            for (var i = 0; i < notUnmutedMuteEvents.Count; i++)
            {
                var muteEvent = notUnmutedMuteEvents[i];
                values.Add($"{i + 1}.",
                    new Dictionary<string, string>
                    {
                        { "Użytkownik:", muteEvent.UserId.GetUserMention() },
                        { "Powód:", muteEvent.Reason },
                        { "Data zakończenia:", muteEvent.TimeRange.End.ToLocalTimeString() }
                    });
            }
            return new MutedUsersMessageData(title, description, values);
        }
    }
}
