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
        public IEnumerable<UserRole> Roles { get; private set; }
        public string AvatarUrl { get; private set; }
        public string Mention { get; private set; }

        public bool IsAdmin => Roles.Any(x => x.Permissions.Any(x => x.HasFlag(Permission.Admin)));
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

    public class UserRole
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<Permission> Permissions { get; private set; }

        public UserRole(ulong id, string name, IEnumerable<Permission> permissions)
        {
            this.Id = id;
            this.Name = name;
            this.Permissions = permissions;
        }
    }

    public enum Permission
    {
        CreateInstantInvite = 1,
        KickMembers = 2,
        BanMembers = 4,
        Administrator = 8,
        ManageChannels = 16,
        ManageGuild = 32,
        AddReactions = 64,
        ViewAuditLog = 128,
        PrioritySpeaker = 256,
        ReadMessages = 1024,
        ViewChannel = 1024,
        SendMessages = 2048,
        SendTTSMessages = 4096,
        ManageMessages = 8192,
        EmbedLinks = 16384,
        AttachFiles = 32768,
        ReadMessageHistory = 65536,
        MentionEveryone = 131072,
        UseExternalEmojis = 262144,
        Connect = 1048576,
        Speak = 2097152,
        MuteMembers = 4194304,
        DeafenMembers = 8388608,
        MoveMembers = 16777216,
        UseVAD = 33554432,
        ChangeNickname = 67108864,
        ManageNicknames = 134217728,
        ManageRoles = 268435456,
        ManageWebhooks = 536870912,
        ManageEmojis = 1073741824
    }
}
