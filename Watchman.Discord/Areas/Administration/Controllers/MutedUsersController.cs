using System.Collections.Generic;
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
            var mutedUsers = _usersService.GetUsersAsync(contexts.Server);
            var (title, description, values) = await this.GetMuteEmbedMessage(mutedUsers, contexts.Server.Id);
            await _directMessagesService.TrySendEmbedMessage(contexts.User.Id, title, description, values);
        }

        private async Task<(string title, string description, Dictionary<string, Dictionary<string, string>> values)> GetMuteEmbedMessage(IAsyncEnumerable<UserContext> mutedUsers, ulong serverId)
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
                            {"Powód ", muteEvent.Reason},
                            {"Data zakończenia ", muteEvent.TimeRange.End.ToLocalTimeString() }
                    });
            }
            return (title, description, values);
        }
    }
}
