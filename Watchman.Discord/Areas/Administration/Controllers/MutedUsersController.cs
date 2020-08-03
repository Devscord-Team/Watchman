using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.Administration.BotCommands;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.DomainModel.Messages;

namespace Watchman.Discord.Areas.Administration.Controllers
{
    public class MutedUsersController : IController
    {
        private readonly MutingHelper _mutingHelper;
        private readonly UsersService _usersService;
        private readonly DirectMessagesService _directMessagesService;

        public MutedUsersController(DirectMessagesService directMessagesService, MutingHelper mutingHelper, UsersService usersService)
        {
            this._directMessagesService = directMessagesService;
            this._mutingHelper = mutingHelper;
            this._usersService = usersService;
        }

        [AdminCommand]
        public async Task MutedUsers(MutedUsersCommand mutedUsersCommand, Contexts contexts)
        {
            var mutedUsersList = await _usersService.GetUsersAsync(contexts.Server).ToListAsync();
            var (title, description, values) = this.GetMuteEmbedMessage(mutedUsersList, contexts.Server.Id);
            await _directMessagesService.TrySendEmbedMessage(contexts.User.Id, title, description, values);
        }
        private (string title, string description, Dictionary<string, Dictionary<string, string>> values) GetMuteEmbedMessage(List<UserContext> mutedUsersList, ulong serverId)
        {
            var title = "Lista wyciszonych użytkowników";
            var description = "Wyciszeni użytkownicy, powody oraz data wygaśnięcia";
            var values = new Dictionary<string, Dictionary<string, string>>();
            foreach (var mutedUser in mutedUsersList)
            {
                var muteEvent = this._mutingHelper.GetNotUnmutedUserMuteEvent(serverId, mutedUser.Id);
                if (muteEvent != null)
                {
                    values.Add($"Użytkownik: {mutedUser.Name}", 
                        new Dictionary<string, string>
                        {
                            {"Powód ", muteEvent.Reason},
                            {"Data zakończenia ", TimeZoneInfo.ConvertTimeFromUtc(muteEvent.TimeRange.End, TimeZoneInfo.Local).ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture) }
                        });
                }
            }
            return (title, description, values);
        }

    }
}
