using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;

namespace Watchman.Discord.Areas.Protection.Services
{
    public interface IPunishmentsCachingService
    {
        Task AddUserPunishment(ulong userId, Punishment punishment);
        int GetUserMutesCount(ulong userId, DateTime? since = null);
        List<Punishment> GetUserPunishments(ulong userId);
        int GetWarnsCount(ulong serverId, ulong userId, DateTime since);
        void AddWarnLocal(ulong grantorId, ulong receiverId, string reason, ulong serverId);
        void RemoveWarnsLocal(ulong serverId, ulong userId, DateTime from);
    }
}
