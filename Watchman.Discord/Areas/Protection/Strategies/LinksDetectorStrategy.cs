using System.Text.RegularExpressions;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;

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

        public SpamProbability GetSpamProbability(ServerMessagesCacheService serverMessagesCacheService, DiscordRequest request, Contexts contexts)
        {
            var content = request.OriginalMessage;
            var linkRegex = new Regex(@"http[s]?:\/\/");
            if (!linkRegex.IsMatch(content))
            {
                return SpamProbability.None;
            }

            var userId = contexts.User.Id;
            var serverId = contexts.Server.Id;
            if (this.UserMessagesCounter.CountUserMessages(userId, serverId) > _userMessagesCountToBeSafe)
            {
                return SpamProbability.Low;
            }
            return SpamProbability.Medium;
        }
    }
}
