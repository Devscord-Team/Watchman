using Devscord.DiscordFramework.Commands.AntiSpam.Models;

namespace Devscord.DiscordFramework.Commands.AntiSpam
{
    public interface ISpamPunishmentStrategy
    {
        public Punishment GetPunishment(ulong userId, SpamProbability spamProbability);
    }
}
