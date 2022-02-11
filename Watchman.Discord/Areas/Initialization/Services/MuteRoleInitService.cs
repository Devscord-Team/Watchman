using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Protection.Complaints.Queries;

namespace Watchman.Discord.Areas.Initialization.Services
{
    public class MuteRoleInitService
    {
        private readonly IQueryBus _queryBus;
        private readonly UsersRolesService _usersRolesService;
        private readonly IChannelsService _channelsService;

        public MuteRoleInitService(IQueryBus queryBus, UsersRolesService usersRolesService, IChannelsService channelsService)
        {
            this._queryBus = queryBus;
            this._usersRolesService = usersRolesService;
            this._channelsService = channelsService;
        }

        public async Task InitForServerAsync(DiscordServerContext server)
        {
            var changedPermissions = this.CreateChangedPermissions();
            var mutedRole = this._usersRolesService.GetRoleByName(UsersRolesService.MUTED_ROLE_NAME, server);
            if (mutedRole == null)
            {
                var createdMutedRole = this.CreateMuteRole(changedPermissions.AllowPermissions);
                mutedRole = await this.SetRoleToServer(server, createdMutedRole);
                await Task.Delay(1000); // wait for complete creating a muted role
            }
            await this.SetChannelsPermissions(server, mutedRole, changedPermissions);
        }

        public Task InitForChannelAsync(ChannelContext channel, DiscordServerContext server)
        {
            var query = new GetComplaintsChannelQuery(server.Id);
            var complaintsChannelId = this._queryBus.Execute(query)?.ComplaintsChannel?.ChannelId;
            if (complaintsChannelId == channel.Id)
            {
                return Task.CompletedTask;
            }
            var changedPermissions = this.CreateChangedPermissions();
            var mutedRole = this._usersRolesService.GetRoleByName(UsersRolesService.MUTED_ROLE_NAME, server);
            if (mutedRole == null)
            {
                return Task.CompletedTask;
            }
            return this._channelsService.SetPermissions(channel, server, changedPermissions, mutedRole);
        }

        private ChangedPermissions CreateChangedPermissions()
        {
            var noPermissions = new List<Permission>();
            var denyPermissions = new List<Permission> { Permission.SendMessages, Permission.SendTTSMessages, Permission.CreateInstantInvite, Permission.AddReactions, Permission.ChangeNickname, Permission.Speak };
            return new ChangedPermissions(noPermissions, denyPermissions);
        }

        private NewUserRole CreateMuteRole(ICollection<Permission> permissions)
        {
            return new NewUserRole(UsersRolesService.MUTED_ROLE_NAME, permissions);
        }

        private Task<UserRole> SetRoleToServer(DiscordServerContext server, NewUserRole mutedRole)
        {
            return this._usersRolesService.CreateNewRole(server, mutedRole);
        }

        private Task SetChannelsPermissions(DiscordServerContext server, UserRole mutedRole, ChangedPermissions changedPermissions)
        {
            return this._channelsService.SetPermissions(server.GetTextChannels(), server, changedPermissions, mutedRole);
        }
    }
}
