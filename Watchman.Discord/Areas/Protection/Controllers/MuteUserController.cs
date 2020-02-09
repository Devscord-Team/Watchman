using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Initialization.Services;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.DomainModel.Mute;
using Watchman.DomainModel.Mute.Commands;
using Watchman.DomainModel.Mute.Queries;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class MuteUserController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly MuteService _muteService;
        private readonly UsersService _usersService;

        public MuteUserController(IQueryBus queryBus, ICommandBus commandBus, MessagesServiceFactory messagesServiceFactory, MuteService muteService, UsersService usersService)
        {
            _queryBus = queryBus;
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

            UnmuteInFuture(contexts, muteEvent, userToMute);
        }

        //[IgnoreForHelp] todo:
        [DiscordCommand("unmute")]
        public void UnmuteUser(DiscordRequest request, Contexts contexts)
        {
            var requestParser = new MuteRequestParser(request, _usersService, contexts);
            var userToUnmute = requestParser.GetUser();

            UnmuteUserOnlyIfMuted(contexts, userToUnmute);
        }

        private async void UnmuteInFuture(Contexts contexts, MuteEvent muteEvent, UserContext userToUnmute)
        {
            var timeRange = muteEvent.TimeRange;
            await Task.Delay(timeRange.End - timeRange.Start);

            UnmuteUserOnlyIfMuted(contexts, userToUnmute);
        }

        private async void UnmuteUserOnlyIfMuted(Contexts contexts, UserContext userToUnmute)
        {
            var getMuteEvents = new GetMuteEventsFromBaseQuery(contexts.Server.Id);
            var allServerMuteEvents = _queryBus.Execute(getMuteEvents).MuteEvents;
            var muteEventToUnmute = allServerMuteEvents.FirstOrDefault(x => x.UserId == userToUnmute.Id);

            if (muteEventToUnmute == null)
            {
                return;
            }

            await _muteService.UnmuteUser(userToUnmute, muteEventToUnmute, contexts.Server);
            var messagesService = _messagesServiceFactory.Create(contexts);
            await messagesService.SendResponse(x => x.UnmutedUser(contexts.User), contexts);
        }
    }
}