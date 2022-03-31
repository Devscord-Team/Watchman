using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Watchman.Cqrs;
using Watchman.DomainModel.Complaints.Queries;
using Watchman.DomainModel.Configuration;

namespace Watchman.Discord.Areas.Administration.Services
{
    public class RolesWithAccessToComplaintsChannelChangesHandler : IConfigurationChangesHandler<IMappedConfiguration>
    {
        private static List<Permission> ReadingAndSending => new() { Permission.ReadMessages, Permission.SendMessages };
        private static ChangedPermissions AccessPermissions => new(allowPermissions: ReadingAndSending, denyPermissions: null);

        private readonly IDiscordServersService _discordServersService;
        private readonly IUsersRolesService _usersRolesService;
        private readonly IChannelsService _channelsService;
        private readonly IQueryBus _queryBus;

        public RolesWithAccessToComplaintsChannelChangesHandler(IQueryBus queryBus, IDiscordServersService discordServersService, IUsersRolesService usersRolesService, IChannelsService channelsService)
        {
            this._discordServersService = discordServersService;
            this._usersRolesService = usersRolesService;
            this._channelsService = channelsService;
            this._queryBus = queryBus;
        }

        public Task Handle(ulong serverId, IMappedConfiguration newConfiguration)
        {
            var query = new GetComplaintsChannelQuery(serverId);
            var complaintsChannelId = this._queryBus.Execute(query).ComplaintsChannel.ChannelId;
            if (complaintsChannelId == 0)
            {
                return Task.CompletedTask;
            }
            return this.RefreshPermissions(newConfiguration, complaintsChannelId);
        }

        private async Task RefreshPermissions(IMappedConfiguration newConfiguration, ulong complaintsChannelId)
        {
            var server = await this._discordServersService.GetDiscordServerAsync(newConfiguration.ServerId);
            var complaintsChannel = server.GetTextChannel(complaintsChannelId);
            var serverRoles = this._usersRolesService.GetRoles(server);
            var conf = (dynamic)newConfiguration;
            var rolesWithAccess = serverRoles.Where(x => conf.Value.Contains(x.Id));
            this.SetAccessPermissions(complaintsChannel, server, rolesWithAccess);
        }

        private void SetAccessPermissions(ChannelContext complaintsChannel, DiscordServerContext server, IEnumerable<UserRole> rolesWithAccess)
        {
            var setAdminPerms = rolesWithAccess.Select(role => this._channelsService.SetPermissions(complaintsChannel, server, AccessPermissions, role));
            Task.WaitAll(setAdminPerms.ToArray());
        }
    }
}
