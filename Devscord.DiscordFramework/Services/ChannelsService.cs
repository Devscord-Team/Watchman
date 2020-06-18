using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Services
{
    public class ChannelsService
    {
        public Task SetPermissions(ChannelContext channel, ChangedPermissions permissions, UserRole userRole) => Server.SetRolePermissions(channel, permissions, userRole);

        public Task SetPermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole userRole) => Server.SetRolePermissions(channels, server, permissions, userRole);
    }
}
