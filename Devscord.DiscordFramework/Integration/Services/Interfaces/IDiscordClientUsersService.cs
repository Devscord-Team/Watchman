using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;

namespace Devscord.DiscordFramework.Integration.Services.Interfaces
{
    public interface IDiscordClientUsersService
    {
        Func<IGuildUser, Task> UserJoined { get; set; }
        Task<IGuildUser> GetGuildUser(ulong userId, ulong guildId);
        Task<bool> IsUserStillOnServer(ulong userId, ulong guildId);
        IEnumerable<IGuildUser> GetGuildUsers(ulong guildId);
        Task<IUser> GetUser(ulong userId);
        RestUser GetBotUser();
    }
}
