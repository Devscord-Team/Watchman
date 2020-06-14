using System.Text.RegularExpressions;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public class LinksDetectorStrategy : ISpamDetector
    {
        public IUserSafetyChecker UserSafetyChecker { get; set; }
        private readonly Regex _linkRegex = new Regex(@"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)", RegexOptions.Compiled);

        public LinksDetectorStrategy(IUserSafetyChecker userSafetyChecker)
        {
            UserSafetyChecker = userSafetyChecker;
        }

        public SpamProbability GetSpamProbability(ServerMessagesCacheService serverMessagesCacheService, DiscordRequest request, Contexts contexts)
        {
            var content = request.OriginalMessage;
            if (!_linkRegex.IsMatch(content))
            {
                return SpamProbability.None;
            }

            var userId = contexts.User.Id;
            var serverId = contexts.Server.Id;
            return this.UserSafetyChecker.IsUserSafe(userId, serverId) 
                ? SpamProbability.Low 
                : SpamProbability.Medium;
        }
    }
}
