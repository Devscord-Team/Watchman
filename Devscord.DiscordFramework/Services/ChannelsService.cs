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
        private readonly Regex exMention = new Regex(@"\d+", RegexOptions.Compiled);
        public Task SetPermissions(ChannelContext channel, DiscordServerContext server, ChangedPermissions permissions, UserRole userRole)
        {
            return Server.SetRolePermissions(channel, server, permissions, userRole);
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
            var match = exMention.Match(mention);
            if (!match.Success)
            {
                Log.Warning("Mention {mention} has not channel ID",mention);
                return null;
            }
            var mentionId = ulong.Parse(match.Value);
            var channel = this.GetChannelById(context, mentionId);
            return channel;
        }
        public ChannelContext GetChannelById(DiscordServerContext context, ulong channelId)
        {
            var channel =  GetTextChannels(context)
                .FirstOrDefault(x => x.Id == channelId);
            if (channel==null)
            {
                Log.Warning("Cannot get channel by id {channelId}",channelId);
            }

            return channel;
        }
    }
}
