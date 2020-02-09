using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Watchman.Common.Models;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class Permissions
    {
        private readonly IEnumerable<Permission> _permissions;
        public ulong RawValue => (ulong)_permissions.Sum(x => (long)x);

        public Permissions()
        {
            this._permissions = new List<Permission>();
        }

        public Permissions(IEnumerable<Permission> permissions)
        {
            this._permissions = permissions;
        }

        public List<Permission> ToList()
        {
            return _permissions.ToList();
        }
    }
}