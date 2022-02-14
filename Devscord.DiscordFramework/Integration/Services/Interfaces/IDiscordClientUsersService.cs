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
        Func<SocketGuildUser, Task> UserJoined { get; set; }
        Task<RestGuildUser> GetGuildUser(ulong userId, ulong guildId);
        Task<bool> IsUserStillOnServer(ulong userId, ulong guildId);
        IEnumerable<IGuildUser> GetGuildUsers(ulong guildId);
        Task<RestUser> GetUser(ulong userId);
        RestUser GetBotUser();
    }
}
