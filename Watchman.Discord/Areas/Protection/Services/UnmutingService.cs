using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Serilog;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Protection.Services.Commands;
using Watchman.DomainModel.Protection.Mutes;
using Watchman.DomainModel.Protection.Mutes.Services;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class UnmutingService : ICyclicService
    {
        private const int SHORT_MUTE_TIME_IN_MINUTES = 15;
        private readonly ICommandBus commandBus;
        private readonly IMutingService mutingService;
        private readonly IUsersService _usersService;
        private readonly IDirectMessagesService _directMessagesService;
        private readonly IMessagesServiceFactory _messagesServiceFactory;
        private readonly IDiscordServersService _discordServersService;
        private readonly HashSet<Guid> _muteEventsAlreadyBeingHandled = new HashSet<Guid>();

        public UnmutingService(ICommandBus commandBus, IMutingService mutingService, IUsersService usersService, IDirectMessagesService directMessagesService, IMessagesServiceFactory messagesServiceFactory, IDiscordServersService discordServersService)
        {
            this.commandBus = commandBus;
            this.mutingService = mutingService;
            this._usersService = usersService;
            this._directMessagesService = directMessagesService;
            this._messagesServiceFactory = messagesServiceFactory;
            this._discordServersService = discordServersService;
        }

        public async Task Refresh()
        {
            Log.Information("Refreshing unmuting...");
            await foreach (var server in this._discordServersService.GetDiscordServersAsync())
            {
                var serverMuteEvents = this.mutingService.GetNotUnmutedMuteEvents(server.Id).ToList();
                if (!serverMuteEvents.Any())
                {
                    continue;
                }
                var contexts = new Contexts();
                contexts.SetContext(server);
                var textChannels = server.GetTextChannels().ToList();
                foreach (var muteEvent in serverMuteEvents.Where(this.ShouldBeConsideredAsShortMute))
                {
                    var user = await this._usersService.GetUserByIdAsync(server, muteEvent.UserId);
                    var channel = textChannels.FirstOrDefault(x => x.Id == muteEvent.MutedOnChannelId);
                    if (user == null)
                    {
                        await this.mutingService.MarkAsUnmuted(muteEvent);
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
            var eventToUnmute = this.mutingService.GetNotUnmutedUserMuteEvent(contexts.Server.Id, userToUnmute.Id);
            if (eventToUnmute == null)
            {
                var messagesService = this._messagesServiceFactory.Create(contexts);
                await messagesService.SendResponse(x => x.UserWasntMuted(userToUnmute));
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
                this._muteEventsAlreadyBeingHandled.Add(muteEvent.Id);
                await Task.Delay(muteEvent.TimeRange.End - DateTime.UtcNow);
            }
            await this.UnmuteSpecificEvent(contexts, userToUnmute, muteEvent);
            this._muteEventsAlreadyBeingHandled.Remove(muteEvent.Id);
        }

        private async Task UnmuteSpecificEvent(Contexts contexts, UserContext userToUnmute, MuteEvent muteEvent)
        {
            await this.commandBus.ExecuteAsync(new UnmuteSpecificEventCommand(contexts, muteEvent, userToUnmute));
            await this.NotifyAboutUnmute(contexts, userToUnmute);
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
