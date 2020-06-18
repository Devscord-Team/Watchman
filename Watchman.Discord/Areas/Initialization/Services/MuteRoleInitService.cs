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

        public Task InitForServer(DiscordServerContext server)
        {
            var changedPermissions = this.CreateChangedPermissions();
            var mutedRole = this.CreateMuteRole(changedPermissions.AllowPermissions);

            var createdRole = this.SetRoleToServer(server, mutedRole);
            this.SetChannelsPermissions(server, createdRole, changedPermissions);
            return Task.CompletedTask;
        }

        private NewUserRole CreateMuteRole(ICollection<Permission> permissions) => new NewUserRole(UsersRolesService.MUTED_ROLE_NAME, permissions);

        private UserRole SetRoleToServer(DiscordServerContext server, NewUserRole mutedRole) => this._usersRolesService.CreateNewRole(server, mutedRole);

        private Task SetChannelsPermissions(DiscordServerContext server, UserRole mutedRole, ChangedPermissions changedPermissions)
        {
            this._channelsService.SetPermissions(server.TextChannels, server, changedPermissions, mutedRole);
            return Task.CompletedTask;
        }

        private ChangedPermissions CreateChangedPermissions()
        {
            var noPermissions = new List<Permission>();
            var denyPermissions = new List<Permission> { Permission.SendMessages, Permission.SendTTSMessages, Permission.CreateInstantInvite };
            return new ChangedPermissions(noPermissions, denyPermissions);
        }
    }
}
