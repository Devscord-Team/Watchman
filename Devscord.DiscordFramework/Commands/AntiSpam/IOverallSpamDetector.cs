using Devscord.DiscordFramework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Commands.AntiSpam
{
    public interface IOverallSpamDetector
    {
        public SpamProbability GetOverallSpamProbability(Contexts contexts);
    }
}
