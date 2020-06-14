using System;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Rest;

namespace Devscord.DiscordFramework.Integration.Services.Interfaces
{
    internal interface IDiscordClientServersService
    {
        Func<SocketGuild, Task> BotAddedToServer { get; set; }
        List<DateTime> ConnectedTimes { get; set; }
        List<DateTime> DisconnectedTimes { get; set; }

        Task<IEnumerable<DiscordServerContext>> GetDiscordServers();
        Task<IEnumerable<string>> GetExistingInviteLinks(ulong serverId);
        Task<RestGuild> GetGuild(ulong guildId);
    }
}
