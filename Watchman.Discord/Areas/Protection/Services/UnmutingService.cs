using System;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Devscord.DiscordFramework.Services.Models;
using Serilog;
using Watchman.DomainModel.Users;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class UnmutingService : ICyclicService
    {
        public RefreshFrequent RefreshFrequent { get; } = RefreshFrequent.Quarterly;
        private const int MINUTES_LEFT_WHEN_MUTE_IS_SHORT = 15;

        private readonly UsersService _usersService;
        private readonly DirectMessagesService _directMessagesService;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly DiscordServersService _discordServersService;
        private readonly MutingHelper _mutingHelper;

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
            foreach (var server in await this._discordServersService.GetDiscordServers())
            {
                var serverMuteEvents = this._mutingHelper.GetServerNotUnmutedMuteEvents(server.Id).ToList();
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
                    var user = this._usersService.GetUserById(server, muteEvent.UserId);
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
        }

        public void UnmuteInFuture(Contexts contexts, MuteEvent muteEvent, UserContext userToUnmute)
        {
            if (this.ShouldBeConsideredAsShortMute(muteEvent))
            {
                this.UnmuteInShortTime(contexts, muteEvent, userToUnmute);
            }
        }

        public async Task UnmuteNow(Contexts contexts, UserContext userToUnmute)
        {
            var serverMuteEvents = this._mutingHelper.GetServerNotUnmutedMuteEvents(contexts.Server.Id);
            var eventToUnmute = serverMuteEvents.FirstOrDefault(x => x.UserId == userToUnmute.Id);
            if (eventToUnmute == null)
            {
                return;
            }
            await this.UnmuteSpecificEvent(contexts, userToUnmute, eventToUnmute);
        }

        private bool ShouldBeConsideredAsShortMute(MuteEvent muteEvent)
        {
            return muteEvent.TimeRange.End < DateTime.UtcNow.AddMinutes(MINUTES_LEFT_WHEN_MUTE_IS_SHORT);
        }

        private async void UnmuteInShortTime(Contexts contexts, MuteEvent muteEvent, UserContext userToUnmute)
        {
            if (muteEvent.TimeRange.End > DateTime.UtcNow)
            {
                await Task.Delay(muteEvent.TimeRange.End - DateTime.UtcNow);
            }
            await this.UnmuteSpecificEvent(contexts, userToUnmute, muteEvent);
        }

        private async Task UnmuteSpecificEvent(Contexts contexts, UserContext userToUnmute, MuteEvent muteEvent)
        {
            var serverMuteEvents = this._mutingHelper.GetServerNotUnmutedMuteEvents(contexts.Server.Id);
            var eventToUnmute = serverMuteEvents.FirstOrDefault(x => x.Id == muteEvent.Id);
            if (eventToUnmute == null)
            {
                return;
            }
            userToUnmute = this._usersService.GetUserById(contexts.Server, userToUnmute.Id);
            if (userToUnmute == null) // user could left the server
            {
                return;
            }
            await this.UnmuteUser(userToUnmute, eventToUnmute, contexts.Server);
            await this.NotifyAboutUnmute(contexts, userToUnmute);
        }

        private async Task UnmuteUser(UserContext mutedUser, MuteEvent muteEvent, DiscordServerContext serverContext)
        {
            var muteRole = this._mutingHelper.GetMuteRole(serverContext);
            await this._usersService.RemoveRole(muteRole, mutedUser, serverContext);
            await this._mutingHelper.MarkAsUnmuted(muteEvent);
            Log.Information("User {user} has been unmuted on server {server}", mutedUser.ToString(), serverContext.Name);
        }

        private async Task NotifyAboutUnmute(Contexts contexts, UserContext unmutedUser)
        {
            if (contexts.Channel == null || contexts.User == null)
            {
                return;
            }
            var messagesService = this._messagesServiceFactory.Create(contexts);
            await messagesService.SendResponse(x => x.UnmutedUser(unmutedUser));
            await this._directMessagesService.TrySendMessage(unmutedUser.Id, x => x.UnmutedUserForUser(unmutedUser, contexts.Server), contexts);
        }
    }
}
