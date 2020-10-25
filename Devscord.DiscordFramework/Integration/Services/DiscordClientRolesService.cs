﻿using System;
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

namespace Devscord.DiscordFramework.Integration.Services
{
    internal class DiscordClientRolesService : IDiscordClientRolesService
    {
        public Func<SocketRole, SocketRole, Task> RoleUpdated { get; set; }
        public Func<SocketRole, Task> RoleCreated { get; set; }
        public Func<SocketRole, Task> RoleRemoved { get; set; }

        private DiscordSocketRestClient _restClient => this._client.Rest;
        private readonly DiscordSocketClient _client;
        private readonly UserRoleFactory _userRoleFactory;
        private List<SocketRole> _roles;

        public DiscordClientRolesService(DiscordSocketClient client, UserRoleFactory userRoleFactory)
        {
            this._client = client;
            this._userRoleFactory = userRoleFactory;
            this._client.Ready += () => Task.Run(() => this._roles = this._client.Guilds.SelectMany(x => x.Roles).ToList());
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

        public IEnumerable<SocketRole> GetSocketRoles(ulong guildId)
        {
            if (this._roles == null)
            {
                this._roles = this._client.Guilds.SelectMany(x => x.Roles).ToList();
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

        private Task AddRole(SocketRole role)
        {
            this._roles.Add(role);
            return Task.CompletedTask;
        }

        private Task RemoveRole(SocketRole role)
        {
            this._roles.Remove(role);
            return Task.CompletedTask;
        }

        private Task UpdateRole(SocketRole from, SocketRole to)
        {
            this._roles.Remove(from);
            this._roles.Add(to);
            return Task.CompletedTask;
        }
    }
}
