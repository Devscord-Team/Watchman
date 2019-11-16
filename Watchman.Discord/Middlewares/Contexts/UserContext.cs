using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Discord.Framework.Architecture.Middlewares;

namespace Watchman.Discord.Middlewares.Contexts
{
    public class UserContext : IDiscordContext
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }
        public bool IsAdmin { get; private set; }
        public IEnumerable<string> Roles { get; private set; }
        public string AvatarUrl { get; private set; }

        public UserContext(ulong id, string name, IEnumerable<string> roles, string avatarUrl)
        {
            Id = id;
            Name = name;
            Roles = roles;
            IsAdmin = roles.Any(x => x.ToLowerInvariant().Contains("admin")); //working with "administrator" etc
            AvatarUrl = avatarUrl;
        }
    }
}
