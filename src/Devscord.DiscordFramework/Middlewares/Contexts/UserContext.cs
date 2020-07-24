using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class UserContext : IDiscordContext
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<UserRole> Roles { get; private set; }
        public string AvatarUrl { get; private set; }
        public string Mention { get; private set; }
        public bool IsAdmin { get; private set; }
        public bool IsMuted { get; private set; }

        public UserContext(ulong id, string name, List<UserRole> roles, string avatarUrl, string mention, bool isOwner)
        {
            this.Id = id;
            this.Name = name;
            this.Roles = roles;
            this.AvatarUrl = avatarUrl;
            this.Mention = mention;
            this.IsAdmin = isOwner || this.Roles.Any(x => x.Permissions.Any(x => x.HasFlag(Permission.ManageGuild)));
            this.IsMuted = this.Roles.Any(x => x.Name == "muted");
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
