using System;
using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Newtonsoft.Json;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class UserContext : IDiscordContext
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<UserRole> Roles { get; private set; }
        public string AvatarUrl { get; private set; }
        public string Mention { get; private set; }
        public bool IsAdmin => this.IsOwner || this.Roles.Any(x => x.Permissions.Any(x => x.HasFlag(Permission.ManageGuild)));
        public bool IsMuted { get; private set; }
        public DateTime? JoinedServerAt => this._getJoinedServerAt.Invoke(this);
        [JsonIgnore] public bool IsOwner => this._isOwner ??= this._getIsOwner.Invoke(this);

        private bool? _isOwner;
        private readonly Func<UserContext, bool> _getIsOwner;
        private readonly Func<UserContext, DateTime?> _getJoinedServerAt;

        public UserContext(ulong id, string name, IReadOnlyCollection<UserRole> roles, string avatarUrl, string mention, Func<UserContext, bool> getIsOwner, Func<UserContext, DateTime?> getJoinedServerAt)
        {
            this.Id = id;
            this.Name = name;
            this.Roles = roles;
            this.AvatarUrl = avatarUrl;
            this.Mention = mention;
            this.IsMuted = this.Roles.Any(x => x.Name == "muted");
            this._getIsOwner = getIsOwner;
            this._getJoinedServerAt = getJoinedServerAt;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
