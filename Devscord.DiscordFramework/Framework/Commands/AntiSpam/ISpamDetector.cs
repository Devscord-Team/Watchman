using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Services.Models;

namespace Devscord.DiscordFramework.Framework.Commands.AntiSpam
{
    public interface ISpamDetector
    {
        public IUserMessagesCounter UserMessagesCounter { get; set; }
        public SpamProbability GetSpamProbability(ServerMessagesCacheService serverMessagesCacheService, Message message);
    }
}
