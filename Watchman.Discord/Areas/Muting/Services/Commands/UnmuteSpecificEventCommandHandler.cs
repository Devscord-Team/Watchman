using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Muting;
using Watchman.DomainModel.Muting.Services;

namespace Watchman.Discord.Areas.Muting.Services.Commands
{
    public class UnmuteSpecificEventCommandHandler : ICommandHandler<UnmuteSpecificEventCommand>
    {
        private readonly IMutingService mutingService;
        private readonly IUsersService usersService;
        private readonly IUsersRolesService usersRolesService;
        private readonly IMessagesServiceFactory messagesServiceFactory;
        private readonly IDirectMessagesService directMessagesService;

        public UnmuteSpecificEventCommandHandler(IMutingService mutingService, IUsersService usersService, IUsersRolesService usersRolesService, 
            IMessagesServiceFactory messagesServiceFactory, IDirectMessagesService directMessagesService)
        {
            this.mutingService = mutingService;
            this.usersService = usersService;
            this.usersRolesService = usersRolesService;
            this.messagesServiceFactory = messagesServiceFactory;
            this.directMessagesService = directMessagesService;
        }

        public async Task HandleAsync(UnmuteSpecificEventCommand command)
        {
            var serverMuteEvents = this.mutingService.GetNotUnmutedMuteEvents(command.Contexts.Server.Id);
            var eventToUnmute = serverMuteEvents.FirstOrDefault(x => x.Id == command.MuteEvent.Id);
            if (eventToUnmute == null)
            {
                return;
            }
            var isStillOnServer = await this.usersService.IsUserStillOnServerAsync(command.Contexts.Server, command.UserToUnmute.Id);
            if (!isStillOnServer)
            {
                return;
            }
            await this.UnmuteUser(command.UserToUnmute, eventToUnmute, command.Contexts.Server);
            await this.NotifyAboutUnmute(command.Contexts, command.UserToUnmute);
        }

        private async Task UnmuteUser(UserContext mutedUser, MuteEvent muteEvent, DiscordServerContext serverContext)
        {
            var muteRole = this.usersRolesService.GetMuteRole(serverContext);
            await this.usersService.RemoveRoleAsync(muteRole, mutedUser, serverContext);
            await this.mutingService.MarkAsUnmuted(muteEvent);
        }

        private async Task NotifyAboutUnmute(Contexts contexts, UserContext unmutedUser)
        {
            if (contexts.Channel == null || contexts.User == null)
            {
                return;
            }
            var messagesService = this.messagesServiceFactory.Create(contexts);
            await messagesService.SendResponse(x => x.UnmutedUser(unmutedUser));
            var (title, description, values) = this.GetUnmuteEmbedMessage(unmutedUser, contexts.Server);
            await this.directMessagesService.TrySendEmbedMessage(unmutedUser.Id, title, description, values);
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
