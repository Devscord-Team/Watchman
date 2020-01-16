using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class UserRole
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }
        public Permissions Permissions { get; private set; }

        public UserRole(string name)
        {
            this.Name = name;
            this.Permissions = new Permissions();
        }

        public UserRole(string name, IEnumerable<Permission> permissions)
        {
            this.Name = name;
            this.Permissions = new Permissions(permissions);
        }

        public UserRole(ulong id, string name, IEnumerable<Permission> permissions)
        {
            this.Id = id;
            this.Name = name;
            this.Permissions = new Permissions(permissions);
        }
    }

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

    public class ChangedPermissions
    {
        public Permissions AllowPermissions { get; }
        public Permissions DenyPermissions { get; }

        public ChangedPermissions(IEnumerable<Permission> allowPermissions, IEnumerable<Permission> denyPermissions)
        {
            AllowPermissions = new Permissions(allowPermissions);
            DenyPermissions = new Permissions(denyPermissions);
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