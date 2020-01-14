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
    class MuteUserController
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly UsersRolesService _usersRolesService;
        private readonly UsersService _usersService;
        private readonly MessagesService _messageService;

        public MuteUserController(IQueryBus queryBus, ICommandBus commandBus, MessagesServiceFactory messageServiceFactory, UsersRolesService usersRolesService, UsersService usersService)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _usersRolesService = usersRolesService;
            _usersService = usersService;
        }

        [DiscordCommand("mute")]
        public void MuteUser(DiscordRequest request, Contexts contexts)
        {
            var userToMute = FindUserByMention(request.Arguments.First().Values.First(), contexts.Server);

            contexts.SetContext(userToMute);
            var muteRole = GetMuteRole(contexts);

            MuteUser(contexts, muteRole);
            _messageService.SendMessage("Użytkownik został zmutowany");
            Task.Delay(10000).Wait();

            UnmuteUser(contexts, muteRole);
            _messageService.SendMessage("Użytkownik może pisać ponownie");
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
