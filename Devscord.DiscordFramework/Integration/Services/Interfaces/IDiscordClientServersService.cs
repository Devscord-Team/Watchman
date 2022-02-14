using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Integration.Services.Interfaces
{
    public interface IDiscordClientServersService
    {
        Func<SocketGuild, Task> BotAddedToServer { get; set; }
        List<DateTime> ConnectedTimes { get; set; }
        List<DateTime> DisconnectedTimes { get; set; }

        IAsyncEnumerable<DiscordServerContext> GetDiscordServersAsync();
        Task<DiscordServerContext> GetDiscordServerAsync(ulong serverId);
        Task<IEnumerable<string>> GetExistingInviteLinks(ulong serverId);
        Task<RestGuild> GetGuild(ulong guildId);
    }
}
