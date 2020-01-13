using System.Linq;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Cqrs;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    class MuteUserController
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly RolesService _rolesService;
        private readonly UsersService _usersService;
        private readonly MessagesService _messageService;

        public MuteUserController(IQueryBus queryBus, ICommandBus commandBus, MessagesServiceFactory messageServiceFactory, RolesService rolesService, UsersService usersService)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _rolesService = rolesService;
            _usersService = usersService;
        }

        [DiscordCommand("mute")]
        public void MuteUser(DiscordRequest request, Contexts contexts)
        {
            var mutedRole = _rolesService.GetRoleByName("muted", contexts.Server);
            var hasUserBeenMutedAlready = contexts.User.Roles.Any(x => x.Name == "muted");

            var userToMute = FindUserByMention(request.Arguments.First().Values.First(), contexts.Server);
            _usersService.AddRole(mutedRole, userToMute, contexts.Server);
        }

        private UserContext FindUserByMention(string mention, DiscordServerContext server)
        {
            return _usersService.GetUsers(server).FirstOrDefault(x => x.Mention == mention);
        }
    }
}
