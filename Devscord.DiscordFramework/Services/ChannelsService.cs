using System;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Middlewares.Factories;

namespace Devscord.DiscordFramework.Services
{
    public class ChannelsService
    {
        public Task SetPermissions(ChannelContext channel, ChangedPermissions permissions, UserRole userRole)
        {
            return Server.SetRolePermissions(channel, permissions, userRole);
        }

        public Task SetPermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole userRole)
        {
            return Server.SetRolePermissions(channels, server, permissions, userRole);
        }
        public IEnumerable<ChannelContext> GetTextChannels(DiscordServerContext server)
        {
            return server.TextChannels;

        }
        public ChannelContext GetChannelByMention(DiscordServerContext context, string mention)
        {
            string processedMention = new string(mention.Where(char.IsNumber).ToArray());
            var mentionId = Convert.ToUInt64(processedMention);
            
            return GetTextChannels(context)
                .FirstOrDefault(x => x.Id == mentionId);
        }
    }
}
