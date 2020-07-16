using System;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var possiblePreviousUserMuteEvent = this.GetNotUnmutedUserMuteEvent(contexts.Server, userToMute);
            var shouldJustMuteAgainTheSameMuteEvent = possiblePreviousUserMuteEvent?.Id == muteEvent.Id;

            if (possiblePreviousUserMuteEvent != null && !shouldJustMuteAgainTheSameMuteEvent)
            {
                var markAsUnmuted = new MarkMuteEventAsUnmutedCommand(possiblePreviousUserMuteEvent.Id);
                await this._commandBus.ExecuteAsync(markAsUnmuted);
            }

            await this.MuteUser(userToMute, contexts.Server);
            await this._commandBus.ExecuteAsync(new AddMuteEventCommand(muteEvent));

            var messagesService = this._messagesServiceFactory.Create(contexts);
            await messagesService.SendResponse(x => x.MutedUser(userToMute, muteEvent.TimeRange.End));
            await this._directMessagesService.TrySendMessage(userToMute.Id, x => x.YouHaveBeenMuted(userToMute, muteEvent.TimeRange.End, muteEvent.Reason), contexts);
        }

        public async Task<bool> UnmuteIfNeeded(DiscordServerContext server, UserContext userToUnmute)
        {
            var userMuteEvents = this.GetUserMuteEvents(server, userToUnmute.Id);
            var eventToUnmute = userMuteEvents.FirstOrDefault(x => x.Unmuted == false);
            if (eventToUnmute == null)
            {
                return false;
            }
            return await this.UnmuteIfNeededSpecificEvent(server, userToUnmute, eventToUnmute);
        }

        public async void UnmuteInFuture(Contexts contexts, MuteEvent muteEvent, UserContext userToUnmute, bool sendMessageAfterUnmute = true)
        {
            if (muteEvent.TimeRange.End > DateTime.UtcNow)
            {
                await Task.Delay(muteEvent.TimeRange.End - DateTime.UtcNow);
            }
            var wasNeededToUnmute = await this.UnmuteIfNeededSpecificEvent(contexts.Server, userToUnmute, muteEvent);
            if (wasNeededToUnmute && sendMessageAfterUnmute)
            {
                var messagesService = this._messagesServiceFactory.Create(contexts);
                await messagesService.SendResponse(x => x.UnmutedUser(userToUnmute));
                await this._directMessagesService.TrySendMessage(userToUnmute.Id, x => x.UnmutedUserForUser(userToUnmute, contexts.Server), contexts);
            }
        }

        public async Task<bool> UnmuteIfNeededSpecificEvent(DiscordServerContext server, UserContext userToUnmute, MuteEvent muteEvent)
        {
            var userMuteEvents = this.GetUserMuteEvents(server, userToUnmute.Id);
            var eventToUnmute = userMuteEvents.FirstOrDefault(x => x.Id == muteEvent.Id);
            if (eventToUnmute == null || eventToUnmute.Unmuted)
            {
                return false;
            }
            userToUnmute = this._usersService.GetUserById(server, userToUnmute.Id);
            if (userToUnmute == null) // user could left the server
            {
                return false;
            }
            await this.UnmuteUser(userToUnmute, eventToUnmute, server);
            return true;
        }

        private MuteEvent GetNotUnmutedUserMuteEvent(DiscordServerContext server, UserContext userContext)
        {
            var userMuteEvents = this.GetUserMuteEvents(server, userContext.Id);
            // in the same time there should exists only one MUTED MuteEvent per user per server
            return userMuteEvents.FirstOrDefault(x => x.Unmuted == false);
        }

        private async Task MuteUser(UserContext userToMute, DiscordServerContext serverContext)
        {
            var muteRole = this.GetMuteRole(serverContext);
            await this.AssignMuteRoleAsync(muteRole, userToMute, serverContext);
        }

        private UserRole GetMuteRole(DiscordServerContext server)
        {
            var muteRole = this._usersRolesService.GetRoleByName(UsersRolesService.MUTED_ROLE_NAME, server);
            if (muteRole == null)
            {
                throw new RoleNotFoundException(UsersRolesService.MUTED_ROLE_NAME);
            }
            return muteRole;
        }

        private async Task AssignMuteRoleAsync(UserRole muteRole, UserContext userToMute, DiscordServerContext server)
        {
            await this._usersService.AddRole(muteRole, userToMute, server);
            Log.Information("User {user} has been muted on server {server}", userToMute.ToString(), server.Name);
        }

        private async Task UnmuteUser(UserContext mutedUser, MuteEvent muteEvent, DiscordServerContext serverContext)
        {
            var muteRole = this.GetMuteRole(serverContext);
            await this._usersService.RemoveRole(muteRole, mutedUser, serverContext);
            await this.MarkAsUnmuted(muteEvent);
            Log.Information("User {user} has been unmuted on server {server}", mutedUser.ToString(), serverContext.Name);
        }

        private async Task MarkAsUnmuted(MuteEvent muteEvent)
        {
            var command = new MarkMuteEventAsUnmutedCommand(muteEvent.Id);
            await this._commandBus.ExecuteAsync(command);
        }

        private IEnumerable<MuteEvent> GetUserMuteEvents(DiscordServerContext server, ulong userId)
        {
            var getMuteEvents = new GetMuteEventsQuery(server.Id);
            var allServerMuteEvents = this._queryBus.Execute(getMuteEvents).MuteEvents;
            return allServerMuteEvents.Where(x => x.UserId == userId);
        }
    }
}
