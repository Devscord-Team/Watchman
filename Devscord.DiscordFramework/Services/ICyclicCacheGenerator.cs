using System.Threading.Tasks;
using Devscord.DiscordFramework.Services.Models;

namespace Devscord.DiscordFramework.Services
{
    public interface ICyclicCacheGenerator
    {
        public RefreshFrequent RefreshFrequent { get; }

        public Task ReloadCache();
    }
}
