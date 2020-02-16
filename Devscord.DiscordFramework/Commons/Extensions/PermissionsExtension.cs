using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Commons.Extensions
{
    internal static class PermissionsExtension
    {
        internal static ulong GetRawValue(this ICollection<Permission> permissions)
        {
            return (ulong)permissions.Sum(x => (long)x);
        }
    }
}
