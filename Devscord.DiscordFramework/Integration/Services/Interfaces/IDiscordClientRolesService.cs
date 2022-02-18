using System;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;

namespace Devscord.DiscordFramework.Integration.Services.Interfaces
{
    public interface IDiscordClientRolesService
    {
        Func<IRole, IRole, Task> RoleUpdated { get; set; }
        Func<IRole, Task> RoleCreated { get; set; }
        Func<IRole, Task> RoleRemoved { get; set; }
        Task<UserRole> CreateNewRole(NewUserRole role, DiscordServerContext discordServer);
        IEnumerable<UserRole> GetRoles(ulong guildId);
        IEnumerable<IRole> GetSocketRoles(ulong guildId);
        UserRole GetRole(ulong roleId, ulong guildId);
    }
}
