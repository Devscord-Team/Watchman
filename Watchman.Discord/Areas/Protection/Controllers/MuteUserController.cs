#nullable enable
using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System.Threading.Tasks;
using Watchman.Discord.Areas.Administration.BotCommands;
using Watchman.Discord.Areas.Administration.Controllers;
using Watchman.Discord.Areas.Administration.Models;
using Watchman.Discord.Areas.Protection.Services;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class MuteUserController : IController
    {
        private readonly MutingHelper _mutingHelper;
        private readonly DirectMessagesService _directMessagesService;
        private readonly MutingService _mutingService;
        private readonly UnmutingService _unmutingService;
        private readonly UsersService _usersService;

        public MuteUserController(MutingService mutingService, UnmutingService unmutingService, UsersService usersService, DirectMessagesService directMessagesService, MutingHelper mutingHelper)
        {
            this._mutingService = mutingService;
            this._unmutingService = unmutingService;
            this._usersService = usersService;
            this._mutingHelper = mutingHelper;
            this._directMessagesService = directMessagesService;
        }

        [DiscordCommand("mute")]
        [AdminCommand]
        public async Task MuteUser(DiscordRequest request, Contexts contexts)
        {
            var requestParser = new MuteRequestParser(request, this._usersService, contexts);
            var userToMute = requestParser.GetUser();
            if (userToMute.Id == this._usersService.GetBot().Id)
            {
                throw new UserDidntMentionAnyUserException();
            }
            var muteEvent = requestParser.GetMuteEvent(userToMute.Id, contexts, request);
            await this._mutingService.MuteUserOrOverwrite(contexts, muteEvent, userToMute);
            this._unmutingService.UnmuteInFuture(contexts, muteEvent, userToMute);
        }

        [DiscordCommand("unmute")]
        [AdminCommand]
        public async Task UnmuteUserAsync(DiscordRequest request, Contexts contexts)
        {
            var requestParser = new MuteRequestParser(request, this._usersService, contexts);
            var userToUnmute = requestParser.GetUser();
            await this._unmutingService.UnmuteNow(contexts, userToUnmute);
        }

        [AdminCommand]
        public async Task MutedUsers(MutedUsersCommand mutedUsersCommand, Contexts contexts)
        {
            var mutedUsers = this._usersService.GetUsersAsync(contexts.Server);
            var mutedUsersMessageData = await this.GetMuteEmbedMessage(mutedUsers, contexts.Server.Id);
            if (!mutedUsersMessageData.Values.Any())
            {
                await this._directMessagesService.TrySendMessage(contexts.User.Id, "Brak wyciszonych użytkowników!");
                return;
            }
            await this._directMessagesService.TrySendEmbedMessage(contexts.User.Id, mutedUsersMessageData.Title, mutedUsersMessageData.Description, mutedUsersMessageData.Values);
        }

        private async Task<MutedUsersMessageData> GetMuteEmbedMessage(IAsyncEnumerable<UserContext> mutedUsers, ulong serverId)
        {
            var title = "Lista wyciszonych użytkowników";
            var description = "Wyciszeni użytkownicy, powody oraz data wygaśnięcia";
            var values = new Dictionary<string, Dictionary<string, string>>();
            await foreach (var mutedUser in mutedUsers)
            {
                var muteEvent = this._mutingHelper.GetNotUnmutedUserMuteEvent(serverId, mutedUser.Id);
                if (muteEvent == null)
                {
                    continue;
                }
                values.Add($"Użytkownik: {mutedUser.Name}",
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
