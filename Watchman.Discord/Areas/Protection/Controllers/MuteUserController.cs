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
using Watchman.DomainModel.Users;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class MuteUserController : IController
    {
        private readonly MutingService _mutingService;
        private readonly UnmutingService _unmutingService;
        private readonly UsersService _usersService;
        private readonly MessagesServiceFactory _messagesServiceFactory;

        public MuteUserController(MutingService mutingService, UnmutingService unmutingService, UsersService usersService, MessagesServiceFactory messagesServiceFactory)
        {
            this._mutingService = mutingService;
            this._unmutingService = unmutingService;
            this._usersService = usersService;
            this._messagesServiceFactory = messagesServiceFactory;
        }

        [AdminCommand]
        public async Task MuteUser(MuteCommand command, Contexts contexts)
        {
            var requestParser = new MuteRequestParser(this._usersService, contexts);
            var userToMute = await requestParser.GetUser(command.Mention);
            MuteEvent? muteEvent = null;
            try
            {
                muteEvent = requestParser.GetMuteEvent(userToMute.Id, contexts, command.Reason, command.Time);
            }
            catch (InvalidArgumentsException)
            {
                var messagesService = this._messagesServiceFactory.Create(contexts);
                await messagesService.SendResponse(x => x.InvalidArguments());
            }
            catch (TimeIsTooBigException)
            {
                var messagesService = this._messagesServiceFactory.Create(contexts);
                await messagesService.SendResponse(x => x.TimeIsTooBig());
            }

            await this._mutingService.MuteUserOrOverwrite(contexts, muteEvent, userToMute);
            this._unmutingService.UnmuteInFuture(contexts, muteEvent, userToMute);
        }

        [AdminCommand]
        public async Task UnmuteUserAsync(UnmuteCommand command, Contexts contexts)
        {
            var requestParser = new MuteRequestParser(this._usersService, contexts);
            var userToUnmute = await requestParser.GetUser(command.Mention);
            await this._unmutingService.UnmuteNow(contexts, userToUnmute);
        }
    }
}
