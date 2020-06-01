using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;

namespace Devscord.DiscordFramework.Framework.Commands.AntiSpam
{
    interface ISpamPunishmentStrategy
    {
        public Punishment GetPunishment(ulong userId, SpamProbability spamProbability);
    }
}
