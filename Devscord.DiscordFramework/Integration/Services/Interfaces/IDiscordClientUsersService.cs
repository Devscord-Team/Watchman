using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Integration.Services.Interfaces
{
    internal interface IDiscordClientUsersService
    {
        Func<SocketGuildUser, Task> UserJoined { get; set; }
        Task<RestGuildUser> GetGuildUser(ulong userId, ulong guildId);
        Task<bool> IsUserStillOnServer(ulong userId, ulong guildId);
        IAsyncEnumerable<RestGuildUser> GetGuildUsers(ulong guildId);
        Task<RestUser> GetUser(ulong userId);
    }
}
