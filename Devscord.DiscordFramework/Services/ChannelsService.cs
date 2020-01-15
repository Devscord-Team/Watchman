using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Services
{
    public class ChannelsService
    {
        public Task SetPermissions(ChannelContext channelContext, ChangedPermissions permissions, UserRole userRole)
        {
            return Server.SetPermissions(channelContext, permissions, userRole);
        }
    }
}
