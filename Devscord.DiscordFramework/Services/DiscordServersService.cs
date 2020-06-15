using System.Collections.Generic;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Services
{
    public class DiscordServersService
    {
        public Task<IEnumerable<DiscordServerContext>> GetDiscordServers()
        {
            return Server.GetDiscordServers();
        }
    }
}
