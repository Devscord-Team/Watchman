using System;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Integration.Services.Interfaces
{
    public interface IDiscordClientRolesService
    {
        Func<SocketRole, SocketRole, Task> RoleUpdated { get; set; }
        Func<SocketRole, Task> RoleCreated { get; set; }
        Func<SocketRole, Task> RoleRemoved { get; set; }
        Task<UserRole> CreateNewRole(NewUserRole role, DiscordServerContext discordServer);
        IEnumerable<UserRole> GetRoles(ulong guildId);
        IEnumerable<SocketRole> GetSocketRoles(ulong guildId);
        UserRole GetRole(ulong roleId, ulong guildId);
    }
}
