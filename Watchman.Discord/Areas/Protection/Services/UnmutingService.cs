using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Serilog;
using Watchman.DomainModel.Users;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class UnmutingService : ICyclicService
    {
        private const int SHORT_MUTE_TIME_IN_MINUTES = 15;

        private readonly UsersService _usersService;
        private readonly DirectMessagesService _directMessagesService;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly DiscordServersService _discordServersService;
        private readonly MutingHelper _mutingHelper;
        private readonly HashSet<Guid> _muteEventsAlreadyBeingHandled = new HashSet<Guid>();

        public UnmutingService(UsersService usersService, DirectMessagesService directMessagesService, MessagesServiceFactory messagesServiceFactory, DiscordServersService discordServersService, MutingHelper mutingHelper)
        {
            this._usersService = usersService;
            this._directMessagesService = directMessagesService;
            this._messagesServiceFactory = messagesServiceFactory;
            this._discordServersService = discordServersService;
            this._mutingHelper = mutingHelper;
        }

        public async Task Refresh()
        {
            Log.Information("Refreshing unmuting...");
            foreach (var server in await this._discordServersService.GetDiscordServers())
            {
                var serverMuteEvents = this._mutingHelper.GetNotUnmutedMuteEvents(server.Id).ToList();
                if (serverMuteEvents.Count == 0)
                {
                    continue;
                }
                var contexts = new Contexts();
                contexts.SetContext(server);
                foreach (var muteEvent in serverMuteEvents)
                {
                    if (!this.ShouldBeConsideredAsShortMute(muteEvent))
                    {
                        continue;
                    }
                    var user = await this._usersService.GetUserByIdAsync(server, muteEvent.UserId);
                    var channel = server.TextChannels.FirstOrDefault(x => x.Id == muteEvent.MutedOnChannelId);
                    if (user == null)
                    {
                        await this._mutingHelper.MarkAsUnmuted(muteEvent);
                        continue;
                    }
                    contexts.SetContext(user);
                    if (channel != null)
                    {
                        contexts.SetContext(channel);
                    }
                    this.UnmuteInShortTime(contexts, muteEvent, user);
                }
            }
            Log.Information("Unmuting refreshed");
        }

        public void UnmuteInFuture(Contexts contexts, MuteEvent muteEvent, UserContext userToUnmute)
        {
            if (this.ShouldBeConsideredAsShortMute(muteEvent))
            {
                this.UnmuteInShortTime(contexts, muteEvent, userToUnmute);
                return;
            }
            Log.Information("Mute {muteEventId} of user {userName} is considered as longer mute", muteEvent.Id, userToUnmute.Name);
        }

        public async Task UnmuteNow(Contexts contexts, UserContext userToUnmute)
        {
            var eventToUnmute = this._mutingHelper.GetNotUnmutedUserMuteEvent(contexts.Server.Id, userToUnmute.Id);
            if (eventToUnmute == null)
            {
                Log.Information("{userName} is not muted", userToUnmute.Name);
                var messagesService = this._messagesServiceFactory.Create(contexts);
                messagesService.SendResponse(x => x.UserWasntMuted(userToUnmute));
                return;
            }
            await this.UnmuteSpecificEvent(contexts, userToUnmute, eventToUnmute);
        }

        private bool ShouldBeConsideredAsShortMute(MuteEvent muteEvent)
        {
            return muteEvent.TimeRange.End < DateTime.UtcNow.AddMinutes(SHORT_MUTE_TIME_IN_MINUTES);
        }

        private async void UnmuteInShortTime(Contexts contexts, MuteEvent muteEvent, UserContext userToUnmute)
        {
            if (this._muteEventsAlreadyBeingHandled.Contains(muteEvent.Id))
            {
                return;
            }
            if (muteEvent.UserId != userToUnmute.Id)
            {
                throw new ArgumentException($"value of {nameof(muteEvent.UserId)} is different than {nameof(userToUnmute.Id)}");
            }
            if (muteEvent.TimeRange.End > DateTime.UtcNow)
            {
                Log.Information("Waiting short time for unmute user {userName} for muteEventId {muteEventId}", userToUnmute.Name, muteEvent.Id);
                this._muteEventsAlreadyBeingHandled.Add(muteEvent.Id);
                await Task.Delay(muteEvent.TimeRange.End - DateTime.UtcNow);
            }
            await this.UnmuteSpecificEvent(contexts, userToUnmute, muteEvent);
            this._muteEventsAlreadyBeingHandled.Remove(muteEvent.Id);
        }

        private async Task UnmuteSpecificEvent(Contexts contexts, UserContext userToUnmute, MuteEvent muteEvent)
        {
            Log.Information("Unmuting {muteEventId} mute event of user {userName}", muteEvent.Id, userToUnmute.Name);
            var serverMuteEvents = this._mutingHelper.GetNotUnmutedMuteEvents(contexts.Server.Id);
            var eventToUnmute = serverMuteEvents.FirstOrDefault(x => x.Id == muteEvent.Id);
            if (eventToUnmute == null)
            {
                Log.Information("Mute event {muteEventId} of user {userName} doesn't exists or was unmuted earlier", muteEvent.Id, userToUnmute.Name);
                return;
            }
            var isStillOnServer = await this._usersService.IsUserStillOnServerAsync(contexts.Server, userToUnmute.Id);
            if (!isStillOnServer)
            {
                Log.Information("User {userName} left server {serverName} so he/she's not going to be unmuted", userToUnmute.Name, contexts.Server.Name);
                return;
            }
            await this.UnmuteUser(userToUnmute, eventToUnmute, contexts.Server);
            await this.NotifyAboutUnmute(contexts, userToUnmute);
        }

        private async Task UnmuteUser(UserContext mutedUser, MuteEvent muteEvent, DiscordServerContext serverContext)
        {
            var muteRole = this._mutingHelper.GetMuteRole(serverContext);
            await this._usersService.RemoveRoleAsync(muteRole, mutedUser, serverContext);
            await this._mutingHelper.MarkAsUnmuted(muteEvent);
            Log.Information("User {user} has been unmuted on server {server}", mutedUser.ToString(), serverContext.Name);
        }

        private async Task NotifyAboutUnmute(Contexts contexts, UserContext unmutedUser)
        {
            if (contexts.Channel == null || contexts.User == null)
            {
                Log.Information("User or channel doesn't exist");
                return;
            }
            var messagesService = this._messagesServiceFactory.Create(contexts);
            await messagesService.SendResponse(x => x.UnmutedUser(unmutedUser));
            var (title, description, values) = this.GetUnmuteEmbedMessage(unmutedUser, contexts.Server);
            await this._directMessagesService.TrySendEmbedMessage(unmutedUser.Id, title, description, values);
        }

        private (string title, string description, IEnumerable<KeyValuePair<string, string>> values) GetUnmuteEmbedMessage(UserContext user, DiscordServerContext server)
        {
            var title = "Wyciszenie wygasło";
            var description = $"Cześć {user.Mention}! Możesz już ponownie pisać na serwerze {server.Name}!";
            var values = new Dictionary<string, string>
            {
                {"Serwer:", server.Name}
            };
            return (title, description, values);
        }
    }
}
