using System.Linq;
using System.Text.RegularExpressions;
using Devscord.DiscordFramework.Commands.AntiSpam;
using Devscord.DiscordFramework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    
    public class LinksDetectorStrategy : ISpamDetector
    {
        private readonly Regex _linkRegex = new Regex(@"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)", RegexOptions.Compiled);
        //private readonly IUserSafetyChecker _userSafetyChecker;

        public LinksDetectorStrategy(/*IUserSafetyChecker userSafetyChecker*/)
        {
            //this._userSafetyChecker = userSafetyChecker;
        }

        public SpamProbability GetSpamProbability(IServerMessagesCacheService serverMessagesCacheService, Contexts contexts)
        {
            var content = serverMessagesCacheService.GetLastUserMessages(contexts.User.Id, contexts.Server.Id).Last().Content;
            if (!this._linkRegex.IsMatch(content))
            {
                return SpamProbability.None;
            }
            var userId = contexts.User.Id;
            var serverId = contexts.Server.Id;
            return SpamProbability.Medium; //todo from configuration
            /*return this._userSafetyChecker.IsUserSafe(userId, serverId)
                ? SpamProbability.Low
                : SpamProbability.Medium;*/
        }
    }
}
