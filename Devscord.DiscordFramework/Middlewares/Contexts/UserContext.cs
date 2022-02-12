﻿using System;
using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Architecture.Middlewares;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class UserContext : IDiscordContext
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }
        public IReadOnlyCollection<UserRole> Roles { get; private set; }
        public string AvatarUrl { get; private set; }
        public string Mention { get; private set; }

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
            this._getIsOwner = getIsOwner;
            this._getJoinedServerAt = getJoinedServerAt;
        }

        public bool IsAdmin() => this.IsOwner() || this.Roles.Any(x => x.Permissions.Any(x => x.HasFlag(Permission.ManageGuild)));
        public bool IsMuted() => this.Roles.Any(x => x.Name == "muted");
        public DateTime? JoinedServerAt() => this._getJoinedServerAt.Invoke(this);
        public bool IsOwner() => this._isOwner ??= this._getIsOwner.Invoke(this);
        public override string ToString() => this.Name;
    }
}
