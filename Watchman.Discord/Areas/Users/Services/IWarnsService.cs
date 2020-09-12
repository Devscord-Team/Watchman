using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.DomainModel.Warns;

namespace Watchman.Discord.Areas.Users.Services
{
    public interface IWarnsService
    {
        Task AddWarnToUser(ulong grantorId, ulong receiverId, string reason, ulong serverId);
        Task RemoveUserWarns(ulong userId, ulong serverId);
        IEnumerable<KeyValuePair<string, string>> GetWarns(UserContext mentionedUser, ulong serverId);
        int GetWarnsCount(ulong userId, ulong serverId, DateTime from);
        IEnumerable<WarnEvent> GetWarnEvents(ulong serverId, ulong userId, DateTime from = new DateTime());
    }
}
