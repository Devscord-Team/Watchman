using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Protection.Services;

namespace Watchman.Discord.Areas.Protection.Factories
{
    public class MuteServiceFactory
    {
        private readonly ICommandBus _commandBus;
        private readonly UsersService _usersService;
        private readonly UsersRolesService _usersRolesService;

        public MuteServiceFactory(ICommandBus commandBus, UsersService usersService, UsersRolesService usersRolesService)
        {
            _commandBus = commandBus;
            this._usersService = usersService;
            this._usersRolesService = usersRolesService;
        }

        public MuteService Create(DiscordRequest request, Contexts contexts)
        {
            return new MuteService(_commandBus, _usersService, _usersRolesService, contexts, request);
        }
    }
}
