using System.Collections.Generic;

namespace Devscord.DiscordFramework.Framework.Commands.AntiSpam
{
    public interface IUserSafetyChecker
    {
        bool IsUserSafe(ulong userId, ulong serverId);
        HashSet<ulong> GetSafeUsersIds(ulong serverId);
    }
}