using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Commons;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class Permissions : ICollection<Permission>
    {
        private readonly ICollection<Permission> _permissions;
        public ulong RawValue => (ulong)_permissions.Sum(x => (long)x);

        public Permissions()
        {
            this._permissions = new List<Permission>();
        }

        public Permissions(ICollection<Permission> permissions)
        {
            this._permissions = permissions;
        }

        public List<Permission> ToList() => _permissions.ToList();
        public IEnumerator<Permission> GetEnumerator() => _permissions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public void Add(Permission permission) => _permissions.Add(permission);
        public void Clear() => _permissions.Clear();
        public bool Contains(Permission permission) => _permissions.Contains(permission);
        public void CopyTo(Permission[] array, int arrayIndex) => _permissions.CopyTo(array, arrayIndex);
        public bool Remove(Permission permission) => _permissions.Remove(permission);
        public int Count => _permissions.Count;
        public bool IsReadOnly => _permissions.IsReadOnly;
    }
}