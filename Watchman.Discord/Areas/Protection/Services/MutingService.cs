using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Protection.Mutes;
using Watchman.DomainModel.Protection.Mutes.Commands;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class MutingService
    {
        private readonly ICommandBus _commandBus;
        private readonly UsersService _usersService;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly DirectMessagesService _directMessagesService;
        private readonly MutingHelper _mutingHelper;

        public MutingService(ICommandBus commandBus, UsersService usersService, MessagesServiceFactory messagesServiceFactory, DirectMessagesService directMessagesService, MutingHelper mutingHelper)
        {
            this._commandBus = commandBus;
            this._usersService = usersService;
            this._messagesServiceFactory = messagesServiceFactory;
            this._directMessagesService = directMessagesService;
            this._mutingHelper = mutingHelper;
        }

        public async Task MuteUserOrOverwrite(Contexts contexts, MuteEvent muteEvent, UserContext userToMute)
        {
            var possiblePreviousUserMuteEvent = this._mutingHelper.GetNotUnmutedUserMuteEvent(contexts.Server.Id, userToMute.Id);
            var shouldJustMuteAgainTheSameMuteEvent = possiblePreviousUserMuteEvent?.Id == muteEvent.Id;

            if (possiblePreviousUserMuteEvent != null && !shouldJustMuteAgainTheSameMuteEvent)
            {
                await this._mutingHelper.MarkAsUnmuted(possiblePreviousUserMuteEvent);
            }
            await this.MuteUser(userToMute, contexts.Server);
            await this._commandBus.ExecuteAsync(new AddMuteEventCommand(muteEvent));
            await this.NotifyUserAboutMute(contexts, userToMute, muteEvent);
        }

        private async Task MuteUser(UserContext userToMute, DiscordServerContext serverContext)
        {
            var muteRole = this._mutingHelper.GetMuteRole(serverContext);
            await this.AssignMuteRoleAsync(muteRole, userToMute, serverContext);
        }

        private async Task AssignMuteRoleAsync(UserRole muteRole, UserContext userToMute, DiscordServerContext server)
        {
            await this._usersService.AddRoleAsync(muteRole, userToMute, server);
            Log.Information("User {user} has been muted on server {server}", userToMute.ToString(), server.Name);
        }

        private async Task NotifyUserAboutMute(Contexts contexts, UserContext mutedUser, MuteEvent muteEvent)
        {   
            var messagesService = this._messagesServiceFactory.Create(contexts);
            await messagesService.SendResponse(x => x.MutedUser(mutedUser, muteEvent.TimeRange.End));
            var (title, description, values) = this.GetMuteEmbedMessage(mutedUser, contexts.Server, muteEvent);
            await this._directMessagesService.TrySendEmbedMessage(mutedUser.Id, title, description, values);
        }

        private (string title, string description, IEnumerable<KeyValuePair<string, string>> values) GetMuteEmbedMessage(UserContext user, DiscordServerContext server, MuteEvent muteEvent)
        {
            var title = "Zostałeś wyciszony";
            var description = $"Cześć {user.Mention}! Zostałeś wyciszony przez moderatora na serwerze {server.Name}";
            var values = new Dictionary<string, string>
            {
                {"Serwer:", server.Name},
                {"Powód:", $"{muteEvent.Reason}"},
                {"Czas wygaśnięcia:", muteEvent.TimeRange.End.ToLocalTimeString()}
            };
            return (title, description, values);
        }
    }
}
