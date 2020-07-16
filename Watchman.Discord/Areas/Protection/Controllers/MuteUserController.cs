#nullable enable
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System.Threading.Tasks;
using Watchman.Discord.Areas.Protection.Services;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class MuteUserController : IController
    {
        private readonly MuteService _muteService;
        private readonly UnmutingService _unmutingService;
        private readonly UsersService _usersService;

        public MuteUserController(MuteService muteService, UnmutingService unmutingService, UsersService usersService)
        {
            this._muteService = muteService;
            this._unmutingService = unmutingService;
            this._usersService = usersService;
        }

        [DiscordCommand("mute")]
        [AdminCommand]
        public async Task MuteUser(DiscordRequest request, Contexts contexts)
        {
            var requestParser = new MuteRequestParser(request, this._usersService, contexts);
            var userToMute = requestParser.GetUser();
            var muteEvent = requestParser.GetMuteEvent(userToMute.Id, contexts, request);

            await this._muteService.MuteUserOrOverwrite(contexts, muteEvent, userToMute);
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
    }
}
