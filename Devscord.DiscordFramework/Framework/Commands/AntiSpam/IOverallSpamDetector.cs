using System.Collections.Generic;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Services.Models;

namespace Devscord.DiscordFramework.Framework.Commands.AntiSpam
{
    public interface IOverallSpamDetector
    {
        protected List<ISpamDetector> SpamDetectors { get; }
        public SpamProbability GetOverallSpamProbability(List<SmallMessage> serverSmallMessages, Message message);
    }
}
