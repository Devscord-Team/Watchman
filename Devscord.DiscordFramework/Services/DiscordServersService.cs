using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Services
{
    public interface IDiscordServersService
    {
        IAsyncEnumerable<DiscordServerContext> GetDiscordServersAsync();
        Task<DiscordServerContext> GetDiscordServerAsync(ulong serverId);
    }

    public class DiscordServersService : IDiscordServersService
    {
        public DiscordServersService()
        {
        }

        public IAsyncEnumerable<DiscordServerContext> GetDiscordServersAsync()
        {
            return Server.GetDiscordServersAsync();
        }

        public Task<DiscordServerContext> GetDiscordServerAsync(ulong serverId)
        {
            return Server.GetDiscordServerAsync(serverId);
        }
    }
}
