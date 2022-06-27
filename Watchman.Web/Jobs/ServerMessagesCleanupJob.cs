using Devscord.DiscordFramework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Services.Models;
using System.Threading.Tasks;

namespace Watchman.Web.Jobs
{
    public class ServerMessagesCleanupJob : IHangfireJob
    {
        public RefreshFrequent Frequency => RefreshFrequent.Minutely;

        public bool RunOnStart => false;

        public Task Do()
        {
            ServerMessagesCacheService.RemoveOldMessagesCyclic();
            return Task.CompletedTask;
        }
    }
}
