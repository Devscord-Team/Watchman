using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Cqrs;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    class MuteUserController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly UsersRolesService _usersRolesService;
        private readonly UsersService _usersService;

        public MuteUserController(IQueryBus queryBus, ICommandBus commandBus, MessagesServiceFactory messagesServiceFactory, UsersRolesService usersRolesService, UsersService usersService)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _messagesServiceFactory = messagesServiceFactory;
            _usersRolesService = usersRolesService;
            _usersService = usersService;
        }

        [DiscordCommand("mute")]
        public void MuteUser(DiscordRequest request, Contexts contexts)
        {
            var userToMute = FindUserByMention(request.Arguments.First().Values.First(), contexts.Server);

            contexts.SetContext(userToMute);
            var muteRole = GetMuteRole(contexts);

            var messagesService = _messagesServiceFactory.Create(contexts);

            MuteUser(contexts, muteRole);
            messagesService.SendMessage("Użytkownik został zmutowany");
            Task.Delay(10000).Wait();

            UnmuteUser(contexts, muteRole);
            messagesService.SendMessage("Użytkownik może pisać ponownie");
        }

        private UserContext FindUserByMention(string mention, DiscordServerContext server)
        {
            return _usersService.GetUsers(server).FirstOrDefault(x => x.Mention == mention);
        }

        private UserRole GetMuteRole(Contexts contexts)
        {
            return _usersRolesService.GetRoleByName("muted", contexts.Server);
        }

        private Task MuteUser(Contexts contexts, UserRole muteRole)
        {
            return _usersService.AddRole(muteRole, contexts.User, contexts.Server);
        }

        private Task UnmuteUser(Contexts contexts, UserRole muteRole)
        {
            return _usersService.RemoveRole(muteRole, contexts.User, contexts.Server);
        }
    }
}
