using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Services.Models;

namespace Devscord.DiscordFramework.Framework.Commands.AntiSpam
{
    public interface IOverallSpamDetector
    {
        public SpamProbability GetOverallSpamProbability(Message message);
    }
}
