using System.Linq;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Users.Services;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    class MuteUserController
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly RolesService _rolesService;
        private readonly MessagesService _messageService;

        public MuteUserController(IQueryBus queryBus, ICommandBus commandBus, MessagesServiceFactory messageServiceFactory, RolesService rolesService)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _rolesService = rolesService;
        }

        [DiscordCommand("mute")]
        public void MuteUser(DiscordRequest request, Contexts contexts)
        {
            bool hasUserBeenMutedAlready = contexts.User.Roles.Any(x => x.Name == "muted");

        }
    }
}
