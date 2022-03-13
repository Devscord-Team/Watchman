using System;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord;
using Discord.WebSocket;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Devscord.DiscordFramework.Integration.Services
{
    [ExcludeFromCodeCoverage]
    internal class DiscordClientRolesService : IDiscordClientRolesService
    {
        public Func<IRole, IRole, Task> RoleUpdated { get; set; }
        public Func<IRole, Task> RoleCreated { get; set; }
        public Func<IRole, Task> RoleRemoved { get; set; }

        private DiscordSocketRestClient _restClient => this._client.Rest;
        private readonly DiscordSocketClient _client;
        private readonly IUserRoleFactory _userRoleFactory;
        private List<IRole> _roles;

        public DiscordClientRolesService(DiscordSocketClient client, IUserRoleFactory userRoleFactory)
        {
            this._client = client;
            this._userRoleFactory = userRoleFactory;
            this._client.Ready += () => Task.Run(() => this._roles = this._client.Guilds.SelectMany(x => x.Roles).Select(x => (IRole)x).ToList());
            this._client.RoleCreated += this.AddRole;
            this._client.RoleCreated += x => this.RoleCreated(x);
            this._client.RoleDeleted += this.RemoveRole;
            this._client.RoleDeleted += x => this.RoleRemoved(x);
            this._client.RoleUpdated += this.UpdateRole;
            this._client.RoleUpdated += (from, to) => this.RoleUpdated(from, to);
        }

        public async Task<UserRole> CreateNewRole(NewUserRole role, DiscordServerContext discordServer)
        {
            var permissionsValue = role.Permissions.GetRawValue();

            var guild = await this._restClient.GetGuildAsync(discordServer.Id);
            var restRole = await guild.CreateRoleAsync(role.Name, new GuildPermissions(permissionsValue), isMentionable: false);
            var userRole = this._userRoleFactory.Create(restRole);

            return userRole;
        }

        public UserRole GetRole(ulong roleId, ulong guildId)
        {
            var restRole = this.GetSocketRoles(guildId).FirstOrDefault(x => x.Id == roleId);
            return restRole == null ? null : this._userRoleFactory.Create(restRole);
        }

        public IEnumerable<IRole> GetSocketRoles(ulong guildId)
        {
            if (this._roles == null)
            {
                this._roles = this._client.Guilds.SelectMany(x => x.Roles).Select(x => (IRole) x).ToList();
            }
            if (this._roles.All(x => x.Guild.Id != guildId))
            {
                this._roles.AddRange(this._client.GetGuild(guildId).Roles);
            }
            return this._roles.Where(x => x.Guild.Id == guildId);
        }

        public IEnumerable<UserRole> GetRoles(ulong guildId)
        {
            return this.GetSocketRoles(guildId).Select(x => this._userRoleFactory.Create(x));
        }

        private Task AddRole(IRole role)
        {
            this._roles.Add(role);
            return Task.CompletedTask;
        }

        private Task RemoveRole(IRole role)
        {
            this._roles.Remove(role);
            return Task.CompletedTask;
        }

        private Task UpdateRole(IRole from, IRole to)
        {
            this._roles.Remove(from);
            this._roles.Add(to);
            return Task.CompletedTask;
        }
    }
}
