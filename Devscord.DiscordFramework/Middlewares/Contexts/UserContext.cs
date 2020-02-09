using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using System.Collections.Generic;
using System.Linq;
using Watchman.Common.Models;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class UserContext : IDiscordContext
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<UserRole> Roles { get; private set; }
        public string AvatarUrl { get; private set; }
        public string Mention { get; private set; }

        public bool IsAdmin => Roles.Any(x => x.Permissions.ToList().Any(x => x.HasFlag(Permission.Administrator)));
        public bool IsMuted => Roles.Any(x => x.Name == "muted");

        public UserContext(ulong id, string name, IEnumerable<UserRole> roles, string avatarUrl, string mention)
        {
            Id = id;
            Name = name;
            Roles = roles;
            AvatarUrl = avatarUrl;
            Mention = mention;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
