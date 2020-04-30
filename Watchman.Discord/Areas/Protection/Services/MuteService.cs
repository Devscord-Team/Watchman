using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Serilog;
using Watchman.Cqrs;
using Watchman.DomainModel.Users;
using Watchman.DomainModel.Users.Commands;
using Watchman.DomainModel.Users.Queries;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class MuteService
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly UsersService _usersService;
        private readonly UsersRolesService _usersRolesService;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly DirectMessagesService _directMessagesService;

        public MuteService(ICommandBus commandBus, IQueryBus queryBus, UsersService usersService, UsersRolesService usersRolesService, MessagesServiceFactory messagesServiceFactory, DirectMessagesService directMessagesService)
        {
            this._commandBus = commandBus;
            this._queryBus = queryBus;
            this._usersService = usersService;
            this._usersRolesService = usersRolesService;
            this._messagesServiceFactory = messagesServiceFactory;
            this._directMessagesService = directMessagesService;
        }

        public async Task MuteUserOrOverwrite(Contexts contexts, MuteEvent muteEvent, UserContext userToMute)
        {
            var possiblePreviousUserMuteEvent = GetNotUnmutedUserMuteEvent(contexts.Server, userToMute);
            var shouldJustMuteAgainTheSameMuteEvent = possiblePreviousUserMuteEvent?.Id == muteEvent.Id;

            if (possiblePreviousUserMuteEvent != null && !shouldJustMuteAgainTheSameMuteEvent)
            {
                var markAsUnmuted = new MarkMuteEventAsUnmutedCommand(possiblePreviousUserMuteEvent.Id);
                await _commandBus.ExecuteAsync(markAsUnmuted);
            }

            await MuteUser(userToMute, contexts.Server);
            await _commandBus.ExecuteAsync(new AddMuteEventCommand(muteEvent));

            var messagesService = _messagesServiceFactory.Create(contexts);
            await messagesService.SendResponse(x => x.MutedUser(userToMute, muteEvent.TimeRange.End), contexts);
        }

        public async Task<bool> UnmuteIfNeeded(DiscordServerContext server, UserContext userToUnmute)
        {
            var userMuteEvents = GetMuteEvents(server, userToUnmute.Id);
            var eventToUnmute = GetNotUnmutedEvent(userMuteEvents);
            if (eventToUnmute == null)
            {
                return false;
            }
            await UnmuteUser(userToUnmute, eventToUnmute, server);
            return true;
        }

        public async void UnmuteInFuture(Contexts contexts, MuteEvent muteEvent, UserContext userToUnmute)
        {
            var timeRange = muteEvent.TimeRange;
            await Task.Delay(timeRange.End - timeRange.Start);

            var wasNeededToUnmute = await UnmuteIfNeeded(contexts.Server, userToUnmute);
            if (wasNeededToUnmute)
            {
                var messagesService = _messagesServiceFactory.Create(contexts);
                await messagesService.SendResponse(x => x.UnmutedUser(userToUnmute), contexts);
                await _directMessagesService.TrySendMessage(userToUnmute.Id, x => x.UnmutedUserForUser(userToUnmute, contexts.Server), contexts);
            }
        }

        private MuteEvent GetNotUnmutedUserMuteEvent(DiscordServerContext server, UserContext userContext)
        {
            var userMuteEvents = GetMuteEvents(server, userContext.Id);
            // in the same time there should exists only one MUTED MuteEvent per user per server
            return userMuteEvents.FirstOrDefault(x => x.Unmuted == false);
        }

        private async Task MuteUser(UserContext userToMute, DiscordServerContext serverContext)
        {
            var muteRole = GetMuteRole(serverContext);
            await AssignMuteRoleAsync(muteRole, userToMute, serverContext);
        }

        private UserRole GetMuteRole(DiscordServerContext server)
        {
            var muteRole = _usersRolesService.GetRoleByName(UsersRolesService.MUTED_ROLE_NAME, server);

            if (muteRole == null)
            {
                throw new RoleNotFoundException(UsersRolesService.MUTED_ROLE_NAME);
            }
            return muteRole;
        }

        private async Task AssignMuteRoleAsync(UserRole muteRole, UserContext userToMute, DiscordServerContext server)
        {
            await _usersService.AddRole(muteRole, userToMute, server);
            Log.Information("User {user} has been muted on server {server}", userToMute.ToString(), server.Name);
        }

        private MuteEvent GetNotUnmutedEvent(IEnumerable<MuteEvent> userMuteEvents)
        {
            return userMuteEvents.FirstOrDefault(x => x.Unmuted == false);
        }

        private async Task UnmuteUser(UserContext mutedUser, MuteEvent muteEvent, DiscordServerContext serverContext)
        {
            var muteRole = GetMuteRole(serverContext);
            await RemoveMuteRoleAsync(muteRole, mutedUser, serverContext);
            await MarkAsUnmuted(muteEvent);
            Log.Information("User {user} has been unmuted on server {server}", mutedUser.ToString(), serverContext.Name);
        }

        private async Task RemoveMuteRoleAsync(UserRole muteRole, UserContext userToUnmute, DiscordServerContext server)
        {
            await _usersService.RemoveRole(muteRole, userToUnmute, server);
        }

        private async Task MarkAsUnmuted(MuteEvent muteEvent)
        {
            var command = new MarkMuteEventAsUnmutedCommand(muteEvent.Id);
            await _commandBus.ExecuteAsync(command);
        }

        private IEnumerable<MuteEvent> GetMuteEvents(DiscordServerContext server, ulong userId)
        {
            var getMuteEvents = new GetMuteEventsQuery(server.Id);
            var allServerMuteEvents = _queryBus.Execute(getMuteEvents).MuteEvents;

            var userMuteEvents = allServerMuteEvents.Where(x => x.UserId == userId);
            return userMuteEvents;
        }
    }
}
