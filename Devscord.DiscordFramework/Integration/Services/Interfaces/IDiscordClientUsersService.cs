using System;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Rest;

namespace Devscord.DiscordFramework.Integration.Services.Interfaces
{
    internal interface IDiscordClientUsersService
    {
        Func<SocketGuildUser, Task> UserJoined { get; set; }
        Task<RestGuildUser> GetGuildUser(ulong userId, ulong guildId);
        Task<IEnumerable<RestGuildUser>> GetGuildUsers(ulong guildId);
        Task<RestUser> GetUser(ulong userId);
    }
}
