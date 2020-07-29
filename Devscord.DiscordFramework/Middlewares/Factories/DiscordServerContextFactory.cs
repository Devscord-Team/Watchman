using Autofac;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Discord;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal class DiscordServerContextFactory : IContextFactory<IGuild, DiscordServerContext>
    {
        private readonly UsersService _usersService;
        private readonly UsersRolesService _usersRolesService;
        private readonly UserContextsFactory _userContextsFactory;
        private readonly ChannelContextFactory _channelContextFactory;

        public DiscordServerContextFactory(IComponentContext context)
        {
            this._usersService = context.Resolve<UsersService>();
            this._usersRolesService = context.Resolve<UsersRolesService>();
            this._userContextsFactory = context.Resolve<UserContextsFactory>();
            this._channelContextFactory = context.Resolve<ChannelContextFactory>();
        }

        public DiscordServerContext Create(IGuild restGuild)
        {
            var systemChannel = this._channelContextFactory.Create(restGuild.GetSystemChannelAsync().Result);
            var textChannels = restGuild.GetTextChannelsAsync().Result.Select(x => this._channelContextFactory.Create(x));

            UserContext GetOwner() => this._userContextsFactory.Create(restGuild.GetOwnerAsync().Result);
            IAsyncEnumerable<UserContext> GetServerUsers(DiscordServerContext server) => this._usersService.GetUsersAsync(server);
            IEnumerable<UserRole> GetServerRoles(DiscordServerContext server) => this._usersRolesService.GetRoles(server);

            return new DiscordServerContext(restGuild.Id, restGuild.Name, GetOwner, systemChannel, textChannels, GetServerUsers, GetServerRoles);
        }
    }
}
