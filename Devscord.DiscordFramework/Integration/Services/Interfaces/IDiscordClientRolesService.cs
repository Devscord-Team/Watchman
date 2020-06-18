using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Integration.Services.Interfaces
{
    internal interface IDiscordClientRolesService
    {
        Task<UserRole> CreateNewRole(NewUserRole role, DiscordServerContext discordServer);
        IEnumerable<UserRole> GetRoles(ulong guildId);
        IEnumerable<SocketRole> GetSocketRoles(ulong guildId);
        Task SetRolePermissions(ChannelContext channel, ChangedPermissions permissions, UserRole role);
        Task SetRolePermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole role);
    }
}
