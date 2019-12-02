using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class UserContext : IDiscordContext
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }
        public bool IsAdmin { get; private set; }
        public IEnumerable<UserRole> Roles { get; private set; }
        public string AvatarUrl { get; private set; }
        public string Mention { get; private set; }

        public UserContext(ulong id, string name, IEnumerable<UserRole> roles, string avatarUrl, string mention)
        {
            Id = id;
            Name = name;
            Roles = roles;
            IsAdmin = roles.Any(x => x.Name.ToLowerInvariant().Contains("admin")); //working with "administrator" etc
            AvatarUrl = avatarUrl;
            Mention = mention;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class UserRole
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }

        public UserRole(ulong id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
