#nullable enable
using System;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.Protection.Services;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class MuteUserController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly MuteService _muteService;
        private readonly UsersService _usersService;
        private readonly DirectMessagesService _directMessagesService;
        public MuteUserController(MessagesServiceFactory messagesServiceFactory, MuteService muteService, UsersService usersService, DirectMessagesService directMessagesService)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._muteService = muteService;
            this._usersService = usersService;
            this._directMessagesService = directMessagesService;
        }

        //[IgnoreForHelp] todo:
        [DiscordCommand("mute")]
        [AdminCommand]
        public async Task MuteUser(DiscordRequest request, Contexts contexts)
        {
            var requestParser = new MuteRequestParser(request, _usersService, contexts);
            var userToMute = requestParser.GetUser();
            var muteEvent = requestParser.GetMuteEvent(userToMute.Id, contexts, request);

            await _muteService.MuteUserOrOverwrite(contexts, muteEvent, userToMute);
            _muteService.UnmuteInFuture(contexts, muteEvent, userToMute);
        }

        //[IgnoreForHelp] todo:
        [DiscordCommand("unmute")]
        [AdminCommand]
        public async Task UnmuteUserAsync(DiscordRequest request, Contexts contexts)
        {
            var requestParser = new MuteRequestParser(request, _usersService, contexts);
            var userToUnmute = requestParser.GetUser();

            var wasMuted = await _muteService.UnmuteIfNeeded(contexts.Server, userToUnmute);
            if (wasMuted)
            {
                var messagesService = _messagesServiceFactory.Create(contexts);
                await messagesService.SendResponse(x => x.UnmutedUser(userToUnmute));
                await _directMessagesService.TrySendMessage(userToUnmute.Id, x => x.UnmutedUserForUser(userToUnmute, contexts.Server), contexts);
            }
        }
    }
}
