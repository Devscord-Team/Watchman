using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Middlewares.Factories;

namespace Devscord.DiscordFramework.Services
{
    public class ChannelsService
    {
        private readonly ChannelContextFactory _channelContextFactory;

        public ChannelsService()
        {
            this._channelContextFactory = new ChannelContextFactory();
        }

        public async Task<ChannelContext> CreateNewChannelAsync(DiscordServerContext server, string channelName)
        {
            var channel = await Server.CreateNewChannel(server.Id, channelName);
            return this._channelContextFactory.Create(channel);
        }

        public Task SetPermissions(ChannelContext channel, DiscordServerContext server, ChangedPermissions permissions, UserRole userRole)
        {
            return Server.SetRolePermissions(channel, server, permissions, userRole);
        }

        public Task SetPermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole userRole)
        {
            return Server.SetRolePermissions(channels, server, permissions, userRole);
        }
    }
}
