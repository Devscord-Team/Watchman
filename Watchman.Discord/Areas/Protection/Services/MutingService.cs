using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Serilog;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Users;
using Watchman.DomainModel.Users.Commands;

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
            var possiblePreviousUserMuteEvent = this.GetNotUnmutedUserMuteEvent(contexts.Server, userToMute);
            var shouldJustMuteAgainTheSameMuteEvent = possiblePreviousUserMuteEvent?.Id == muteEvent.Id;

            if (possiblePreviousUserMuteEvent != null && !shouldJustMuteAgainTheSameMuteEvent)
            {
                await this._mutingHelper.MarkAsUnmuted(possiblePreviousUserMuteEvent);
            }
            await this.MuteUser(userToMute, contexts.Server);
            await this._commandBus.ExecuteAsync(new AddMuteEventCommand(muteEvent));

            var messagesService = this._messagesServiceFactory.Create(contexts);
            await messagesService.SendResponse(x => x.MutedUser(userToMute, muteEvent.TimeRange.End));
            await this._directMessagesService.TrySendMessage(userToMute.Id, x => x.YouHaveBeenMuted(userToMute, muteEvent.TimeRange.End, muteEvent.Reason), contexts);
        }

        private MuteEvent GetNotUnmutedUserMuteEvent(DiscordServerContext server, UserContext userContext)
        {
            var serverMuteEvents = this._mutingHelper.GetServerNotUnmutedMuteEvents(server.Id);
            // in the same time there should exists only one MUTED MuteEvent per user per server
            return serverMuteEvents.FirstOrDefault(x => x.UserId == userContext.Id);
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
    }
}
