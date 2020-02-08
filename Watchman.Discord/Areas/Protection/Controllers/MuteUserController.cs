using System.Threading;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Initialization.Services;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.DomainModel.Mute.Commands;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class MuteUserController : IController
    {
        private readonly ICommandBus _commandBus;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly MuteService _muteService;
        private readonly UsersService _usersService;

        public MuteUserController(ICommandBus commandBus, MessagesServiceFactory messagesServiceFactory, MuteService muteService, UsersService usersService)
        {
            this._commandBus = commandBus;
            this._messagesServiceFactory = messagesServiceFactory;
            this._muteService = muteService;
            _usersService = usersService;
        }

        //[IgnoreForHelp] todo:
        [DiscordCommand("mute")]
        public void MuteUser(DiscordRequest request, Contexts contexts)
        {
            var messagesService = _messagesServiceFactory.Create(contexts);

            var requestParser = new MuteRequestParser(request, _usersService, contexts);
            var userToMute = requestParser.GetUser();

            _muteService.MuteUser(userToMute, contexts.Server).Wait();

            var muteEvent = requestParser.GetMuteEvent(userToMute.Id, contexts);
            _commandBus.ExecuteAsync(new AddMuteEventToBaseCommand(muteEvent));
            messagesService.SendResponse(x => x.MutedUser(userToMute, muteEvent.TimeRange.End), contexts);

            var timeRange = muteEvent.TimeRange;
            Thread.Sleep(timeRange.End - timeRange.Start);

            _muteService.UnmuteUser(userToMute, muteEvent, contexts.Server).Wait();
            messagesService.SendResponse(x => x.UnmutedUser(contexts.User), contexts);
        }
    }
}