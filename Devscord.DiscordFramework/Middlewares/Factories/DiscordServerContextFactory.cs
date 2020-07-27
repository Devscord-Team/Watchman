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
        private readonly UserContextsFactory _userContextsFactory;
        private readonly ChannelContextFactory _channelContextFactory;

        public DiscordServerContextFactory(UsersService usersService, UserContextsFactory userContextsFactory, ChannelContextFactory channelContextFactory)
        {
            this._usersService = usersService;
            this._userContextsFactory = userContextsFactory;
            this._channelContextFactory = channelContextFactory;
        }

        public DiscordServerContext Create(IGuild restGuild)
        {
            var owner = this._userContextsFactory.Create(restGuild.GetOwnerAsync().Result);
            var systemChannel = this._channelContextFactory.Create(restGuild.GetSystemChannelAsync().Result);
            var textChannels = restGuild.GetTextChannelsAsync().Result.Select(x => this._channelContextFactory.Create(x));

            IAsyncEnumerable<UserContext> GetServerUsers(DiscordServerContext server) => this._usersService.GetUsersAsync(server);
            return new DiscordServerContext(restGuild.Id, restGuild.Name, owner, systemChannel, textChannels, GetServerUsers);
        }
    }
}
