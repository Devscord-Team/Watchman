using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Protection.Mutes;
using Watchman.DomainModel.Protection.Mutes.Services;

namespace Watchman.Discord.Areas.Protection.Services.Commands
{
    public class UnmuteSpecificEventCommand : ICommand
    {
        public Contexts Contexts { get; }
        public MuteEvent MuteEvent { get; }
        public UserContext UserToUnmute { get; }

        public UnmuteSpecificEventCommand(Contexts contexts, MuteEvent muteEvent, UserContext UserToUnmute)
        {
            this.Contexts = contexts;
            this.MuteEvent = muteEvent;
            this.UserToUnmute = UserToUnmute;
        }
    }

    public class UnmuteSpecificEventCommandHandler : ICommandHandler<UnmuteSpecificEventCommand>
    {
        private readonly IMutingService mutingService;
        private readonly IUsersService usersService;
        private readonly IUsersRolesService usersRolesService;

        public UnmuteSpecificEventCommandHandler(IMutingService mutingService, IUsersService usersService, IUsersRolesService usersRolesService)
        {
            this.mutingService = mutingService;
            this.usersService = usersService;
            this.usersRolesService = usersRolesService;
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
        }

        private async Task UnmuteUser(UserContext mutedUser, MuteEvent muteEvent, DiscordServerContext serverContext)
        {
            var muteRole = this.usersRolesService.GetMuteRole(serverContext);
            await this.usersService.RemoveRoleAsync(muteRole, mutedUser, serverContext);
            await this.mutingService.MarkAsUnmuted(muteEvent);
        }
    }
}
