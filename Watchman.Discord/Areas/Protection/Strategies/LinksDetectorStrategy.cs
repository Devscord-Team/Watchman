using System.Text.RegularExpressions;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Services.Models;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public class LinksDetectorStrategy : ISpamDetector
    {
        public IUserMessagesCounter UserMessagesCounter { get; set; }

        private readonly int _userMessagesCountToBeSafe;
        
        public LinksDetectorStrategy(IUserMessagesCounter userMessagesCounter)
        {
            UserMessagesCounter = userMessagesCounter;
            _userMessagesCountToBeSafe = 500;
        }

        public SpamProbability GetSpamProbability(ServerMessagesCacheService serverMessagesCacheService, Message message)
        {
            var content = message.Request.OriginalMessage;
            var linkRegex = new Regex(@"http[s]?:\/\/");
            if (!linkRegex.IsMatch(content))
            {
                return SpamProbability.None;
            }

            var userId = message.Contexts.User.Id;
            var serverId = message.Contexts.Server.Id;
            if (this.UserMessagesCounter.CountUserMessages(userId, serverId) > _userMessagesCountToBeSafe)
            {
                return SpamProbability.Low;
            }
            return SpamProbability.Medium;
        }
    }
}
