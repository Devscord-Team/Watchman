using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Architecture.Controllers;
using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Common.Models;
using Watchman.Discord.Areas.Protection.BotCommands;
using Watchman.Discord.Areas.Protection.Commands;
using Watchman.Discord.Areas.Protection.Models;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.DomainModel.Protection.Mutes;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class MuteUserController : IController
    {
        private readonly MutingHelper _mutingHelper;
        private readonly DirectMessagesService _directMessagesService;
        private readonly MutingService _mutingService;
        private readonly UnmutingService _unmutingService;
        private readonly UsersService _usersService;
        private readonly MessagesServiceFactory _messagesServiceFactory;

        public MuteUserController(MutingService mutingService, UnmutingService unmutingService, UsersService usersService, DirectMessagesService directMessagesService, MutingHelper mutingHelper, MessagesServiceFactory messagesServiceFactory)
        {
            this._mutingService = mutingService;
            this._unmutingService = unmutingService;
            this._usersService = usersService;
            this._directMessagesService = directMessagesService;
            this._mutingHelper = mutingHelper;
            this._messagesServiceFactory = messagesServiceFactory;
        }

        [AdminCommand]
        public async Task MuteUser(MuteCommand command, Contexts contexts)
        {
            var userToMute = await this._usersService.GetUserByIdAsync(contexts.Server, command.User);
            if (userToMute == null || userToMute.Id == this._usersService.GetBot().Id)
            {
                throw new UserNotFoundException(command.User.GetUserMention());
            }
            var timeRange = TimeRange.FromNow(DateTime.UtcNow + command.Time); //todo: change DateTime.UtcNow to Contexts.SentAt
            var muteEvent = new MuteEvent(userToMute.Id, timeRange, command.Reason, contexts.Server.Id, contexts.Channel.Id);
            await this._mutingService.MuteUserOrOverwrite(contexts, muteEvent, userToMute);
            this._unmutingService.UnmuteInFuture(contexts, muteEvent, userToMute);
        }

        [AdminCommand]
        public async Task UnmuteUserAsync(UnmuteCommand command, Contexts contexts)
        {
            var userToUnmute = await this._usersService.GetUserByIdAsync(contexts.Server, command.User);
            if (userToUnmute == null)
            {
                throw new UserNotFoundException(command.User.GetUserMention());
            }
            await this._unmutingService.UnmuteNow(contexts, userToUnmute);
        }

        [AdminCommand]
        public async Task MutedUsers(MutedUsersCommand mutedUsersCommand, Contexts contexts)
        {
            var notUnmutedMuteEvents = this._mutingHelper.GetNotUnmutedMuteEvents(contexts.Server.Id);
            var messagesService = this._messagesServiceFactory.Create(contexts);
            var mutedUsersMessageData = this.GetMuteEmbedMessage(notUnmutedMuteEvents.ToList());
            if (!mutedUsersMessageData.Values.Any())
            {
                await messagesService.SendResponse(x => x.ThereAreNoMutedUsers());
                return;
            }
            await this._directMessagesService.TrySendEmbedMessage(contexts.User.Id, mutedUsersMessageData.Title, mutedUsersMessageData.Description, mutedUsersMessageData.Values);
            await messagesService.SendResponse(x => x.MutedUsersListSent());
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
