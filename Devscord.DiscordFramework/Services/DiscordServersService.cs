using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Services
{
    public class DiscordServersService
    {
        public IAsyncEnumerable<DiscordServerContext> GetDiscordServersAsync()
        {
            return Server.GetDiscordServersAsync();
        }

        public Task<DiscordServerContext> GetDiscordServerAsync(ulong serverId)
        {
            return Server.GetDiscordServerAsync(serverId);
        }

        public Task<ulong[]> GetUsersIdsFromServer(ulong serverId)
        {
            return Server.GetUsersIdsFromServer(serverId);
        }
    }
}
