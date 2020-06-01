using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Services.Models;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public class LinksDetectorStrategy : ISpamDetector
    {
        private readonly int _userMessagesCountToBeSafe;

        public LinksDetectorStrategy()
        {
            _userMessagesCountToBeSafe = 500;
        }

        public SpamProbability GetSpamProbability(List<SmallMessage> serverSmallMessages, Message message)
        {
            var content = message.Request.OriginalMessage;
            var linkRegex = new Regex(@"http[s]?:\/\/");
            if (!linkRegex.IsMatch(content))
            {
                return SpamProbability.None;
            }

            var userId = message.Contexts.User.Id;
            if (serverSmallMessages.Count(x => x.UserId == userId) > _userMessagesCountToBeSafe)
            {
                return SpamProbability.Low;
            }
            return SpamProbability.Medium;
        }
    }
}
