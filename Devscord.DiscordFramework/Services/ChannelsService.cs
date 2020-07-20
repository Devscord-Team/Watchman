using System;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Middlewares.Factories;
using Serilog;

namespace Devscord.DiscordFramework.Services
{
    public class ChannelsService
    {
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
