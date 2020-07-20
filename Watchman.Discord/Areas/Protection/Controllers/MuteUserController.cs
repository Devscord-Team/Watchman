#nullable enable
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Discord.Areas.Protection.BotCommands;
using Watchman.Discord.Areas.Protection.Services;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class MuteUserController : IController
    {
        private readonly MutingService _mutingService;
        private readonly UnmutingService _unmutingService;
        private readonly UsersService _usersService;

        public MuteUserController(MutingService mutingService, UnmutingService unmutingService, UsersService usersService)
        {
            this._mutingService = mutingService;
            this._unmutingService = unmutingService;
            this._usersService = usersService;
        }

        [AdminCommand]
        public async Task MuteUser(MuteCommand command, Contexts contexts)
        {
            var requestParser = new MuteRequestParser(this._usersService, contexts);
            var userToMute = requestParser.GetUser(command.Users?.FirstOrDefault(x => x.StartsWith('<') && x.EndsWith('>')));
            var muteEvent = requestParser.GetMuteEvent(userToMute.Id, contexts, command.Reasons?.FirstOrDefault(), command.Times?.FirstOrDefault());

            await this._mutingService.MuteUserOrOverwrite(contexts, muteEvent, userToMute);
            this._unmutingService.UnmuteInFuture(contexts, muteEvent, userToMute);
        }

        [AdminCommand]
        public async Task UnmuteUserAsync(UnmuteCommand command, Contexts contexts)
        {
            var requestParser = new MuteRequestParser(this._usersService, contexts);
            var userToUnmute = requestParser.GetUser(command.Users?.FirstOrDefault(x => x.StartsWith('<') && x.EndsWith('>')));
            await this._unmutingService.UnmuteNow(contexts, userToUnmute);
        }
    }
}
