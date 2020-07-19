#nullable enable
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Responses;
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

        [AdminCommand]
        public async Task MuteUser(MuteCommand command, Contexts contexts)
        {
            var requestParser = new MuteRequestParser(this._usersService, contexts);
            var userToMute = requestParser.GetUser(command.Users?.FirstOrDefault(x => x.StartsWith('<') && x.EndsWith('>')));
            var muteEvent = requestParser.GetMuteEvent(userToMute.Id, contexts, command.Reasons?.FirstOrDefault(), command.Times?.FirstOrDefault());

            await this._muteService.MuteUserOrOverwrite(contexts, muteEvent, userToMute);
            this._muteService.UnmuteInFuture(contexts, muteEvent, userToMute);
        }

        [AdminCommand]
        public async Task UnmuteUserAsync(UnmuteCommand command, Contexts contexts)
        {
            var requestParser = new MuteRequestParser(this._usersService, contexts);
            var userToUnmute = requestParser.GetUser(command.Users?.FirstOrDefault(x => x.StartsWith('<') && x.EndsWith('>')));

            var wasMuted = await this._muteService.UnmuteIfNeeded(contexts.Server, userToUnmute);
            if (wasMuted)
            {
                var messagesService = this._messagesServiceFactory.Create(contexts);
                await messagesService.SendResponse(x => x.UnmutedUser(userToUnmute));
                await this._directMessagesService.TrySendMessage(userToUnmute.Id, x => x.UnmutedUserForUser(userToUnmute, contexts.Server), contexts);
            }
        }
    }
}
