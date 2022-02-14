using Autofac;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Discord;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    public interface IDiscordServerContextFactory : IContextFactory<IGuild, DiscordServerContext>
    {
    }

    internal class DiscordServerContextFactory : IDiscordServerContextFactory
    {
        private readonly IUsersService _usersService;
        private readonly IUsersRolesService _usersRolesService;
        private readonly IUserContextsFactory _userContextsFactory;
        private readonly IChannelContextFactory _channelContextFactory;

        public DiscordServerContextFactory(IUsersService usersService, IUsersRolesService usersRolesService, 
            IUserContextsFactory userContextsFactory, IChannelContextFactory channelContextFactory)
        {
            this._usersService = usersService;
            this._usersRolesService = usersRolesService;
            this._userContextsFactory = userContextsFactory;
            this._channelContextFactory = channelContextFactory;
        }

        public DiscordServerContext Create(IGuild guild)
        {
            var systemChannel = this._channelContextFactory.Create(guild.GetSystemChannelAsync().Result);

            UserContext getOwner() => this._userContextsFactory.Create(guild.GetOwnerAsync().Result);
            IEnumerable<UserContext> getServerUsers(DiscordServerContext server) => this._usersService.GetUsers(server);
            IEnumerable<UserRole> getServerRoles(DiscordServerContext server) => this._usersRolesService.GetRoles(server);
            IEnumerable<ChannelContext> getTextChannels(DiscordServerContext server) => guild.GetTextChannelsAsync().Result.Select(x => this._channelContextFactory.Create(x));

            return new DiscordServerContext(guild.Id, guild.Name, getOwner, systemChannel, getTextChannels, getServerUsers, getServerRoles);
        }
    }
}
