using System.Collections.Generic;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;

namespace Watchman.Discord.Areas.Initialization.Services
{
    public class MuteRoleInitService
    {
        private readonly UsersRolesService _usersRolesService;
        private readonly ChannelsService _channelsService;

        public MuteRoleInitService(UsersRolesService usersRolesService, ChannelsService channelsService)
        {
            _usersRolesService = usersRolesService;
            _channelsService = channelsService;
        }

        public async Task InitForServer(DiscordServerContext server)
        {
            var changedPermissions = CreateChangedPermissions();
            var mutedRole = CreateMuteRole(changedPermissions.AllowPermissions);

            var createdRole = await SetRoleToServer(server, mutedRole);
            await SetChannelsPermissions(server, createdRole, changedPermissions);
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
            await this._channelsService.SetPermissions(server.TextChannels, server, changedPermissions, mutedRole);
        }

        private ChangedPermissions CreateChangedPermissions()
        {
            var noPermissions = new List<Permission>();
            var denyPermissions = new List<Permission> { Permission.SendMessages, Permission.SendTTSMessages, Permission.CreateInstantInvite };
            return new ChangedPermissions(noPermissions, denyPermissions);
        }
    }
}
