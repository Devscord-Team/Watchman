using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;

namespace Devscord.DiscordFramework.Framework.Commands.AntiSpam
{
    public interface ISpamPunishmentStrategy
    {
        public Punishment GetPunishment(ulong userId, ulong serverId, SpamProbability spamProbability);
    }
}
