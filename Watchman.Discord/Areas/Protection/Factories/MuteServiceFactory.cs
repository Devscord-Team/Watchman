using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.Protection.Services;

namespace Watchman.Discord.Areas.Protection.Factories
{
    public class MuteServiceFactory
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly UsersService _usersService;
        private readonly UsersRolesService _usersRolesService;

        public MuteServiceFactory(MessagesServiceFactory messagesServiceFactory, UsersService usersService, UsersRolesService usersRolesService)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._usersService = usersService;
            this._usersRolesService = usersRolesService;
        }

        public MuteService Create(Contexts contexts)
        {
            return new MuteService(_messagesServiceFactory, _usersService, _usersRolesService, contexts);
        }
    }
}
