using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Watchman.Discord.Areas.Initialization.Services
{
    public class MuteRoleInitService
    {
        private readonly UsersRolesService _usersRolesService;
        private readonly ChannelsService _channelsService;

        public MuteRoleInitService(UsersRolesService usersRolesService, ChannelsService channelsService)
        {
            this._usersRolesService = usersRolesService;
            this._channelsService = channelsService;
        }

        public async Task InitForServer(DiscordServerContext server)
        {
            var mutedRole = this._usersRolesService.GetRoleByName(UsersRolesService.MUTED_ROLE_NAME, server);
            var changedPermissions = this.CreateChangedPermissions();
            if (mutedRole == null)
            {
                var createdMutedRole = this.CreateMuteRole(changedPermissions.AllowPermissions);
                mutedRole = await this.SetRoleToServer(server, createdMutedRole);
            }
            await this.SetChannelsPermissions(server, mutedRole, changedPermissions);
        }

        private NewUserRole CreateMuteRole(ICollection<Permission> permissions)
        {
            return new NewUserRole(UsersRolesService.MUTED_ROLE_NAME, permissions);
        }

        private async Task<UserRole> SetRoleToServer(DiscordServerContext server, NewUserRole mutedRole)
        {
            return await this._usersRolesService.CreateNewRole(server, mutedRole);
        }

        private async Task SetChannelsPermissions(DiscordServerContext server, UserRole mutedRole, ChangedPermissions changedPermissions)
        {
            await this._channelsService.SetPermissions(server.GetTextChannels(), server, changedPermissions, mutedRole);
        }

        private ChangedPermissions CreateChangedPermissions()
        {
            var noPermissions = new List<Permission>();
            var denyPermissions = new List<Permission> { Permission.SendMessages, Permission.SendTTSMessages, Permission.CreateInstantInvite };
            return new ChangedPermissions(noPermissions, denyPermissions);
        }
    }
}
