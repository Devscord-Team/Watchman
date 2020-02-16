#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Cqrs;
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
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._messagesServiceFactory = messagesServiceFactory;
            this._muteService = muteService;
            this._usersService = usersService;
        }

        //[IgnoreForHelp] todo:
        [DiscordCommand("mute")]
        public void MuteUser(DiscordRequest request, Contexts contexts)
        {
            var requestParser = new MuteRequestParser(request, _usersService, contexts);
            var userToMute = requestParser.GetUser();
            var muteEvent = requestParser.GetMuteEvent(userToMute.Id, contexts);

            MuteUserOrOverwrite(contexts, muteEvent, userToMute).Wait();
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

        private async Task MuteUserOrOverwrite(Contexts contexts, MuteEvent muteEvent, UserContext userToMute)
        {
            var possiblePreviousUserMuteEvent = GetNotUnmutedUserMuteEvent(contexts.Server, userToMute);
            if (possiblePreviousUserMuteEvent != null)
            {
                var markAsUnmuted = new MarkMuteEventAsUnmutedCommand(possiblePreviousUserMuteEvent);
                await _commandBus.ExecuteAsync(markAsUnmuted);
            }

            await _muteService.MuteUser(userToMute, contexts.Server);
            await _commandBus.ExecuteAsync(new AddMuteEventToBaseCommand(muteEvent));

            var messagesService = _messagesServiceFactory.Create(contexts);
            await messagesService.SendResponse(x => x.MutedUser(userToMute, muteEvent.TimeRange.End), contexts);
        }

        private MuteEvent? GetNotUnmutedUserMuteEvent(DiscordServerContext server, UserContext userContext)
        {
            var userMuteEvents = GetMuteEvents(server, userContext.Id);
            // in the same time there should exists only one MUTED MuteEvent per user per server
            return userMuteEvents.FirstOrDefault(x => x.Unmuted == false);
        }

        private IEnumerable<MuteEvent> GetMuteEvents(DiscordServerContext server, ulong userId)
        {
            var getMuteEvents = new GetMuteEventsFromBaseQuery(server.Id);
            var allServerMuteEvents = _queryBus.Execute(getMuteEvents).MuteEvents;

            var userMuteEvents = allServerMuteEvents.Where(x => x.UserId == userId);
            return userMuteEvents;
        }

        private async void UnmuteInFuture(Contexts contexts, MuteEvent muteEvent, UserContext userToUnmute)
        {
            var timeRange = muteEvent.TimeRange;
            await Task.Delay(timeRange.End - timeRange.Start);

            if (IsStillMuted(contexts.Server, muteEvent))
            {
                UnmuteUserOnlyIfMuted(contexts, userToUnmute);
            }
        }

        private bool IsStillMuted(DiscordServerContext server, MuteEvent muteEvent)
        {
            var userMuteEvents = GetMuteEvents(server, muteEvent.UserId);
            return userMuteEvents.Any(x => x.Unmuted == false);
        }

        private async void UnmuteUserOnlyIfMuted(Contexts contexts, UserContext userToUnmute)
        {
            var muteEventToUnmute = GetNotUnmutedUserMuteEvent(contexts.Server, userToUnmute);
            if (muteEventToUnmute == null)
            {
                return;
            }

            await _muteService.UnmuteUser(userToUnmute, muteEventToUnmute, contexts.Server);

            var messagesService = _messagesServiceFactory.Create(contexts);
            await messagesService.SendResponse(x => x.UnmutedUser(userToUnmute), contexts);
        }
    }
}