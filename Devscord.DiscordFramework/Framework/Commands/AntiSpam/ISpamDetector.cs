using System.Collections.Generic;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Services.Models;

namespace Devscord.DiscordFramework.Framework.Commands.AntiSpam
{
    public interface ISpamDetector
    {
        public SpamProbability GetSpamProbability(List<SmallMessage> serverSmallMessages, Message message);
    }
}
