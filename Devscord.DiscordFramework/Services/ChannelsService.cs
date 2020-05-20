using System.Collections.Generic;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Services
{
    public class ChannelsService
    {
        public Task SetPermissions(ChannelContext channel, ChangedPermissions permissions, UserRole userRole)
        {
            return Server.SetPermissions(channel, permissions, userRole);
        }

        public Task SetPermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole userRole)
        {
            return Server.SetPermissions(channels, server, permissions, userRole);
        }
    }
}
