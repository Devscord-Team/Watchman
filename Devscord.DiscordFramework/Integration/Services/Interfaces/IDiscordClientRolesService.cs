using System;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Rest;

namespace Devscord.DiscordFramework.Integration.Services.Interfaces
{
    internal interface IDiscordClientRolesService
    {
        Func<SocketRole, SocketRole, Task> RoleUpdated { get; set; }
        Func<SocketRole, Task> RoleCreated { get; set; }
        Func<SocketRole, Task> RoleRemoved { get; set; }
        Task<UserRole> CreateNewRole(NewUserRole role, DiscordServerContext discordServer);
        IEnumerable<UserRole> GetRoles(ulong guildId);
        IEnumerable<SocketRole> GetSocketRoles(ulong guildId);
        Task SetRolePermissions(ChannelContext channel, DiscordServerContext server, ChangedPermissions permissions, UserRole role);
        Task SetRolePermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole role);
        UserRole GetRole(ulong roleId, ulong guildId);
    }
}
