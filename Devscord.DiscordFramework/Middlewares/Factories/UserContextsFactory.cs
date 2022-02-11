using Autofac;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal class UserContextsFactory : IContextFactory<IUser, UserContext>
    {
        private readonly IUsersRolesService _usersRolesService;
        private readonly DiscordServersService _discordServersService;

        public UserContextsFactory(IUsersRolesService usersRolesService, DiscordServersService discordServersService)
        {
            this._usersRolesService = usersRolesService;
            this._discordServersService = discordServersService;
        }

        public UserContext Create(IUser user)
        {
            var avatarUrl = user.GetAvatarUrl(ImageFormat.Png, 2048);
            if (!(user is IGuildUser guildUser))
            {
                return new UserContext(user.Id, user.ToString(), new List<UserRole>(), avatarUrl, user.Mention, getIsOwner: _ => false, getJoinedServerAt: _ => null);
            }
            var roles = guildUser.RoleIds.Select(x => this._usersRolesService.GetRole(x, guildUser.GuildId));
            bool getIsOwner(UserContext userContext) => userContext.Id == this._discordServersService.GetDiscordServerAsync(guildUser.GuildId).Result.GetOwner().Id;
            DateTime? getJoinedServerAt(UserContext userContext) => guildUser.JoinedAt?.DateTime;

            return new UserContext(user.Id, user.ToString(), roles.ToList(), avatarUrl, user.Mention, getIsOwner, getJoinedServerAt);
        }
    }
}
