using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Services
{
    public interface ICyclicCacheGenerator
    {
        public Task ReloadCache();
    }
}
