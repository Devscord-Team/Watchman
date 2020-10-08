using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Watchman.Cqrs;
using Watchman.DomainModel.Configuration;
using Watchman.DomainModel.Configuration.ConfigurationItems;
using Watchman.DomainModel.Configuration.Services;
using Watchman.DomainModel.Protection.Commands;
using Watchman.DomainModel.Protection.Queries;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class ComplaintsChannelService
    {
        private static List<Permission> ReadingAndSending => new List<Permission> {Permission.ReadMessages, Permission.SendMessages};
        private static ChangedPermissions AccessPermissions => new ChangedPermissions(allowPermissions: ReadingAndSending, denyPermissions: null);

        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly IConfigurationService _configurationService;
        private readonly ChannelsService _channelsService;
        private readonly UsersRolesService _usersRolesService;
        private readonly DiscordServersService _discordServersService;

        public ComplaintsChannelService(IQueryBus queryBus, ICommandBus commandBus, IConfigurationService configurationService, ChannelsService channelsService, UsersRolesService usersRolesService, DiscordServersService discordServersService)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._configurationService = configurationService;
            this._channelsService = channelsService;
            this._usersRolesService = usersRolesService;
            this._discordServersService = discordServersService;
        }

        public Task RemoveIfNeededComplaintsChannel(ChannelContext channel, DiscordServerContext server)
        {
            var query = new GetComplaintsChannelQuery(server.Id);
            var complaintsChannel = this._queryBus.Execute(query).ComplaintsChannel;
            if (channel.Id != complaintsChannel?.ChannelId)
            {
                return Task.CompletedTask;
            }
            var command = new RemoveComplaintsChannelCommand(complaintsChannel);
            return this._commandBus.ExecuteAsync(command);
        }

        public async Task<ChannelContext> CreateComplaintsChannel(string channelName, Contexts contexts)
        {
            if (this.IsComplaintsChannelAlreadyExisting(contexts.Server.Id))
            {
                throw new ComplaintsChannelAlreadyExistsException();
            }
            var complaintsChannel = await this._channelsService.CreateNewChannelAsync(contexts.Server, channelName);
            complaintsChannel = contexts.Server.GetTextChannel(complaintsChannel.Id);
            await this.SetChannelPermissions(complaintsChannel, contexts);
            await this.NotifyDomainAboutComplaintsChannel(complaintsChannel, contexts.Server);
            return complaintsChannel;
        }

        private bool IsComplaintsChannelAlreadyExisting(ulong serverId)
        {
            var query = new GetComplaintsChannelQuery(serverId);
            var complaintsChannelId = this._queryBus.Execute(query).ComplaintsChannel?.ChannelId;
            return complaintsChannelId.HasValue && complaintsChannelId.Value != 0;
        }

        private async Task SetChannelPermissions(ChannelContext channel, Contexts contexts)
        {
            var everyonePermissions = new ChangedPermissions(allowPermissions: null, denyPermissions: ReadingAndSending);
            var mutedPermissions = new ChangedPermissions(ReadingAndSending, new List<Permission> { Permission.ReadMessageHistory });

            var serverRoles = this._usersRolesService.GetRoles(contexts.Server).ToList();
            var everyoneRole = serverRoles.First(x => x.Name == "@everyone");
            var mutedRole = serverRoles.FirstOrDefault(x => x.Name == UsersRolesService.MUTED_ROLE_NAME);

            var rolesIdsWithAccess = this._configurationService.GetConfigurationItem<RolesWithAccessToComplaintsChannel>(contexts.Server.Id);
            var rolesWithAccess = rolesIdsWithAccess.Value == null 
                ? contexts.Server.GetRoles().Where(x => x.Permissions.Contains(Permission.ManageGuild)).ToList() 
                : rolesIdsWithAccess.Value.Select(roleId => serverRoles.FirstOrDefault(serverRole => roleId == serverRole.Id)).ToList();
            rolesWithAccess.Add(mutedRole);

            await Task.Delay(1500);
            await this._channelsService.RemovePermissions(channel, contexts.Server, mutedRole);
            await this._channelsService.SetPermissions(channel, contexts.Server, mutedPermissions, mutedRole);
            this.SetAccessPermissions(channel, contexts.Server, rolesWithAccess);
            await this._channelsService.SetPermissions(channel, contexts.Server, everyonePermissions, everyoneRole);
        }

        private void SetAccessPermissions(ChannelContext complaintsChannel, DiscordServerContext server, IEnumerable<UserRole> rolesWithAccess)
        {
            var setAdminPerms = rolesWithAccess.Select(role => this._channelsService.SetPermissions(complaintsChannel, server, AccessPermissions, role));
            Task.WaitAll(setAdminPerms.ToArray());
        }

        private Task NotifyDomainAboutComplaintsChannel(ChannelContext channel, DiscordServerContext server)
        {
            var addComplaintsChannelCommand = new AddComplaintsChannelCommand(channel.Id, server.Id);
            return this._commandBus.ExecuteAsync(addComplaintsChannelCommand);
        }
    }
}
