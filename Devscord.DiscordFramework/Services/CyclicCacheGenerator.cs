using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Services
{
    public enum RefreshFrequent
    {
        Minutely = 1,
        Quarterly = 2,
        Hourly = 4,
        Daily = 8,
        Weekly = 16,
        Monthly = 32,
        Yearly = 64
    }

    public interface ICyclicCacheGenerator
    {
        public RefreshFrequent RefreshFrequent { get; }

        public Task ReloadCache();
    }
}
