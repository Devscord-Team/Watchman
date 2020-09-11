using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Watchman.Common.Models;
using Watchman.Discord.Areas.Protection.BotCommands;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.Protection.Commands;
using Watchman.Discord.Areas.Protection.Models;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.DomainModel.Users;

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
            if (userToMute == null)
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
            var notUnmutedMuteEvents = _mutingHelper.GetNotUnmutedMuteEvents(contexts.Server.Id);
            var mutedUsers = this._usersService.GetUsersAsync(contexts.Server);
            var mutedUsersMessageData = await this.GetMuteEmbedMessage(notUnmutedMuteEvents.ToList(), mutedUsers);
            if (!mutedUsersMessageData.Values.Any())
            {
                await this._directMessagesService.TrySendMessage(contexts.User.Id, "Brak wyciszonych użytkowników!");
                return;
            }
            await this._directMessagesService.TrySendEmbedMessage(contexts.User.Id, mutedUsersMessageData.Title, mutedUsersMessageData.Description, mutedUsersMessageData.Values);
            var messagesService = this._messagesServiceFactory.Create(contexts);
            await messagesService.SendResponse(x => x.MutedUsersListSent());
        }

        private async Task<MutedUsersMessageData> GetMuteEmbedMessage(IEnumerable<MuteEvent> notUnmutedMuteEvents, IAsyncEnumerable<UserContext> users)
        {
            var title = "Lista wyciszonych użytkowników";
            var description = "Wyciszeni użytkownicy, powody oraz data wygaśnięcia";
            var values = new Dictionary<string, Dictionary<string, string>>();
            await foreach (var user in users)
            {
                var muteEvent = notUnmutedMuteEvents.FirstOrDefault(x => x.UserId == user.Id);
                if (muteEvent == null)
                {
                    continue;
                }
                values.Add($"Użytkownik: {user.Name}",
                    new Dictionary<string, string>
                    {
                        {"Powód:", muteEvent.Reason},
                        {"Data zakończenia:", muteEvent.TimeRange.End.ToLocalTimeString() }
                    });
            }
            return new MutedUsersMessageData(title, description, values);
        }
    }
}
