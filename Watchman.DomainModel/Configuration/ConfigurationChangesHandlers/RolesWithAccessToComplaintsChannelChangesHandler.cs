using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Watchman.Cqrs;
using Watchman.DomainModel.Configuration.ConfigurationItems;
using Watchman.DomainModel.Protection.Queries;

namespace Watchman.DomainModel.Configuration.ConfigurationChangesHandlers
{
    public class RolesWithAccessToComplaintsChannelChangesHandler : IConfigurationChangesHandler<RolesWithAccessToComplaintsChannel>
    {
        private static List<Permission> ReadingAndSending => new List<Permission> {Permission.ReadMessages, Permission.SendMessages};
        private static ChangedPermissions AccessPermissions => new ChangedPermissions(allowPermissions: ReadingAndSending, denyPermissions: null);

        private readonly DiscordServersService _discordServersService;
        private readonly UsersRolesService _usersRolesService;
        private readonly ChannelsService _channelsService;
        private readonly IQueryBus _queryBus;

        public RolesWithAccessToComplaintsChannelChangesHandler(IQueryBus queryBus, DiscordServersService discordServersService, UsersRolesService usersRolesService, ChannelsService channelsService)
        {
            this._discordServersService = discordServersService;
            this._usersRolesService = usersRolesService;
            this._channelsService = channelsService;
            this._queryBus = queryBus;
        }

        public Task Handle(ulong serverId, RolesWithAccessToComplaintsChannel newConfiguration)
        {
            var query = new GetComplaintsChannelQuery(serverId);
            var complaintsChannelId = this._queryBus.Execute(query).ComplaintsChannel.ChannelId;
            if (complaintsChannelId == 0)
            {
                return Task.CompletedTask;
            }
            return this.RefreshPermissions(newConfiguration, complaintsChannelId);
        }

        private async Task RefreshPermissions(RolesWithAccessToComplaintsChannel newConfiguration, ulong complaintsChannelId)
        {
            var server = await this._discordServersService.GetDiscordServerAsync(newConfiguration.ServerId);
            var complaintsChannel = server.GetTextChannel(complaintsChannelId);
            var serverRoles = this._usersRolesService.GetRoles(server);
            var rolesWithAccess = serverRoles.Where(x => newConfiguration.Value.Contains(x.Id));
            this.SetAccessPermissions(complaintsChannel, server, rolesWithAccess);
        }

        private void SetAccessPermissions(ChannelContext complaintsChannel, DiscordServerContext server, IEnumerable<UserRole> rolesWithAccess)
        {
            var setAdminPerms = rolesWithAccess.Select(role => this._channelsService.SetPermissions(complaintsChannel, server, AccessPermissions, role));
            Task.WaitAll(setAdminPerms.ToArray());
        }
    }
}
