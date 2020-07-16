using System.Threading.Tasks;
using Devscord.DiscordFramework.Services.Models;

namespace Devscord.DiscordFramework.Services
{
    public interface ICyclicService
    {
        public RefreshFrequent RefreshFrequent { get; }

        public Task Refresh();
    }
}
