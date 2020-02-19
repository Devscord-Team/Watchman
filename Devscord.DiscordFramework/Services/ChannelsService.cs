using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Services
{
    public class ChannelsService
    {
        public Task SetPermissions(ChannelContext channel, ChangedPermissions permissions, UserRole userRole)
        {
            return Server.SetPermissions(channel, permissions, userRole);
        }
    }
}
