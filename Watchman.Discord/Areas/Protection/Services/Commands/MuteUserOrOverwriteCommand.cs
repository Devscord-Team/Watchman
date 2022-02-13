using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Protection.Mutes;
using Watchman.DomainModel.Protection.Mutes.Commands;
using Watchman.DomainModel.Protection.Mutes.Services;

namespace Watchman.Discord.Areas.Protection.Services.Commands
{
    public class MuteUserOrOverwriteCommand : ICommand
    {
        public Contexts Contexts { get; }
        public MuteEvent MuteEvent { get; }
        public UserContext UserToMute { get; }

        public MuteUserOrOverwriteCommand(Contexts contexts, MuteEvent muteEvent, UserContext userToMute)
        {
            this.Contexts = contexts;
            this.MuteEvent = muteEvent;
            this.UserToMute = userToMute;
        }
    }

    public class MuteUserOrOverwriteCommandHandler : ICommandHandler<MuteUserOrOverwriteCommand>
    {
        private readonly ICommandBus commandBus;
        private readonly IUsersService usersService;
        private readonly IMessagesServiceFactory messagesServiceFactory;
        private readonly IDirectMessagesService directMessagesService;
        private readonly IMutingService mutingService;
        private readonly IUsersRolesService usersRolesService;

        public MuteUserOrOverwriteCommandHandler(ICommandBus commandBus, IUsersService usersService, IMessagesServiceFactory messagesServiceFactory,
            IDirectMessagesService directMessagesService, IMutingService mutingService, IUsersRolesService usersRolesService)
        {
            this.commandBus = commandBus;
            this.usersService = usersService;
            this.messagesServiceFactory = messagesServiceFactory;
            this.directMessagesService = directMessagesService;
            this.mutingService = mutingService;
            this.usersRolesService = usersRolesService;
        }

        public async Task HandleAsync(MuteUserOrOverwriteCommand command)
        {
            if (await this.mutingService.ShouldUserBeMuted(command.MuteEvent) == false)
            {
                return;
            }
            var muteRole = this.usersRolesService.GetMuteRole(command.Contexts.Server);
            await this.usersService.AddRoleAsync(muteRole, command.UserToMute, command.Contexts.Server);
            await this.commandBus.ExecuteAsync(new AddMuteEventCommand(command.MuteEvent));
            await this.NotifyUserAboutMute(command.Contexts, command.UserToMute, command.MuteEvent);
        }

        private async Task NotifyUserAboutMute(Contexts contexts, UserContext mutedUser, MuteEvent muteEvent)
        {
            var messagesService = this.messagesServiceFactory.Create(contexts);
            await messagesService.SendResponse(x => x.MutedUser(mutedUser, muteEvent.TimeRange.End));
            var (title, description, values) = this.GetMuteEmbedMessage(mutedUser, contexts.Server, muteEvent);
            await this.directMessagesService.TrySendEmbedMessage(mutedUser.Id, title, description, values);
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
