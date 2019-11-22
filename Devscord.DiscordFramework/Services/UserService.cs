using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Services
{
    public class UserService
    {
        public Task AddRole(ulong roleId, Dictionary<string, IDiscordContext> contexts)
        {
            var user = GetUser(contexts);
            var role = GetRole(roleId, user);
            return user.AddRoleAsync(role);
        }

        public Task RemoveRole(ulong roleId, Dictionary<string, IDiscordContext> contexts)
        {
            var user = GetUser(contexts);
            var role = GetRole(roleId, user);
            return user.RemoveRoleAsync(role);
        }


        private SocketGuildUser GetUser(Dictionary<string, IDiscordContext> contexts)
        {
            var userId = ((UserContext)contexts[nameof(UserContext)]).Id;
            var guildId = ((DiscordServerContext)contexts[nameof(DiscordServerContext)]).Id;

            return Server.GetGuildUser(userId, guildId);
        }

        private SocketRole GetRole(ulong roleId, ChannelContext channel)
        {
            return Server.GetRoles(channel.Id).First(x => x.Id == roleId);
        }
    }
}
