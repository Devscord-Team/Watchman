using System;
using System.Collections.Generic;
using System.Text;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;

namespace Watchman.Discord.Areas.Initialization.Services
{
    public class MutedRoleInitService
    {
        private readonly UsersRolesService _usersRolesService;
        private readonly ChannelsService _channelsService;

        public MutedRoleInitService(UsersRolesService usersRolesService, ChannelsService channelsService)
        {
            _usersRolesService = usersRolesService;
            _channelsService = channelsService;
        }

        public void InitForServer(Contexts contexts)
        {
            var changedPermissions = CreateChangedPermissions();
            var mutedRole = CreateMuteRole(changedPermissions.AllowPermissions);

            var createdRole = SetRoleToServer(contexts, mutedRole);
            SetChannelsPermissions(contexts, createdRole, changedPermissions);
        }

        private UserRole CreateMuteRole(Permissions permissions)
        {
            return new UserRole(UsersRolesService.MUTED_ROLE_NAME, permissions.ToList());
        }

        private UserRole SetRoleToServer(Contexts contexts, UserRole mutedRole)
        {
            return _usersRolesService.CreateNewRole(contexts, mutedRole);
        }

        private void SetChannelsPermissions(Contexts contexts, UserRole mutedRole, ChangedPermissions changedPermissions)
        {
            foreach (var channel in contexts.Server.TextChannels)
            {
                _channelsService.SetPermissions(contexts.Server, channel, changedPermissions, mutedRole);
            }
        }

        private ChangedPermissions CreateChangedPermissions()
        {
            var onlyReadPermission = new List<Permission>();
            var denyPermissions = new List<Permission> { Permission.SendMessages, Permission.SendTTSMessages, Permission.CreateInstantInvite };
            return new ChangedPermissions(onlyReadPermission, denyPermissions);
        }
    }
}
