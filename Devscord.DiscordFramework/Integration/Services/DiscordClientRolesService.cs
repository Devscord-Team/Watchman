using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Integration.Services
{
    internal class DiscordClientRolesService : IDiscordClientRolesService
    {
        private DiscordSocketRestClient _restClient => this._client.Rest;
        private readonly DiscordSocketClient _client;
        private readonly IDiscordClientChannelsService _discordClientChannelsService;
        private List<SocketRole> _roles;

        public DiscordClientRolesService(DiscordSocketClient client, IDiscordClientChannelsService discordClientChannelsService)
        {
            this._client = client;
            this._discordClientChannelsService = discordClientChannelsService;

            this._client.Ready += async () => await Task.Run(() =>
            {
                return this._roles = this._client.Guilds.SelectMany(x => x.Roles).ToList();
            });
            this._client.RoleCreated += this.AddRole;
            this._client.RoleDeleted += this.RemoveRole;
            this._client.RoleUpdated += this.RoleUpdated;
        }

        public async Task<UserRole> CreateNewRole(NewUserRole role, DiscordServerContext discordServer)
        {
            var permissionsValue = role.Permissions.GetRawValue();

            var guild = await this._restClient.GetGuildAsync(discordServer.Id);
            var restRole = await guild.CreateRoleAsync(role.Name, new GuildPermissions(permissionsValue), isMentionable: false);
            var userRole = new UserRoleFactory().Create(restRole);

            return userRole;
        }

        public async Task SetRolePermissions(ChannelContext channel, ChangedPermissions permissions, UserRole role)
        {
            await Task.Delay(1000);

            var channelSocket = (IGuildChannel) await this._discordClientChannelsService.GetChannel(channel.Id);
            var channelPermissions = new OverwritePermissions(permissions.AllowPermissions.GetRawValue(), permissions.DenyPermissions.GetRawValue());
            var createdRole = this.GetSocketRoles(channelSocket.GuildId).FirstOrDefault(x => x.Id == role.Id);

            await channelSocket.AddPermissionOverwriteAsync(createdRole, channelPermissions);
        }

        public async Task SetRolePermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole role)
        {
            await Task.Delay(1000);
            var createdRole = this.GetSocketRoles(server.Id).FirstOrDefault(x => x.Id == role.Id);
            var channelPermissions = new OverwritePermissions(permissions.AllowPermissions.GetRawValue(), permissions.DenyPermissions.GetRawValue());

            Parallel.ForEach(channels, async c =>
            {
                var channelSocket = (IGuildChannel) await this._discordClientChannelsService.GetChannel(c.Id);
                await channelSocket.AddPermissionOverwriteAsync(createdRole, channelPermissions);
            });
        }

        public IEnumerable<SocketRole> GetSocketRoles(ulong guildId)
        {
            if (this._roles == null) // todo: it should work without this if
            {
                this._roles = this._client.Guilds.SelectMany(x => x.Roles).ToList();
            }

            return this._roles.Where(x => x.Guild.Id == guildId);
        }

        public IEnumerable<UserRole> GetRoles(ulong guildId)
        {
            var roleFactory = new UserRoleFactory();
            return this.GetSocketRoles(guildId).Select(x => roleFactory.Create(x));
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

        private Task RoleUpdated(SocketRole from, SocketRole to)
        {
            this._roles.Remove(from);
            this._roles.Add(to);
            return Task.CompletedTask;
        }
    }
}
