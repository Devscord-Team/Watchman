using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public class SpamPunishmentStrategy : ISpamPunishmentStrategy
    {
        public Punishment GetPunishment(ulong userId, SpamProbability spamProbability)
        {
            throw new System.NotImplementedException();
        }
    }
}
