using Devscord.DiscordFramework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Commands.AntiSpam
{
    public interface ISpamDetector
    {
        public SpamProbability GetSpamProbability(ServerMessagesCacheService serverMessagesCacheService, Contexts contexts);
    }
}
