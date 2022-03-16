﻿using Discord;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watchman.Discord.IntegrationTests.TestEnvironment.Models
{
    internal class FakeUser : IUser, IGuildUser
    {
        public string AvatarId { get; set; }

        public string Discriminator { get; set; }

        public ushort DiscriminatorValue { get; set; }

        public bool IsBot { get; set; }

        public bool IsWebhook { get; set; }

        public string Username { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public ulong Id { get; set; }

        public string Mention { get; set; }

        public IActivity Activity { get; set; }

        public UserStatus Status { get; set; }

        public IImmutableSet<ClientType> ActiveClients { get; set; }

        public DateTimeOffset? JoinedAt { get; set; }

        public string Nickname { get; set; }

        public GuildPermissions GuildPermissions { get; set; }

        public IGuild Guild { get; set; }

        public ulong GuildId { get; set; } = 1;

        public DateTimeOffset? PremiumSince { get; set; }

        public IReadOnlyCollection<ulong> RoleIds { get; set; } = new List<ulong>() { 1, 2, 3 };

        public bool IsDeafened { get; set; }

        public bool IsMuted { get; set; }

        public bool IsSelfDeafened { get; set; }

        public bool IsSelfMuted { get; set; }

        public bool IsSuppressed { get; set; }

        public IVoiceChannel VoiceChannel { get; set; }

        public string VoiceSessionId { get; set; }

        public bool IsStreaming { get; set; }

        public UserProperties? PublicFlags => throw new NotImplementedException();

        public IReadOnlyCollection<IActivity> Activities => throw new NotImplementedException();

        public string DisplayName => throw new NotImplementedException();

        public string DisplayAvatarId => throw new NotImplementedException();

        public string GuildAvatarId => throw new NotImplementedException();

        public bool? IsPending => throw new NotImplementedException();

        public int Hierarchy => throw new NotImplementedException();

        public DateTimeOffset? TimedOutUntil => throw new NotImplementedException();

        public bool IsVideoing => throw new NotImplementedException();

        public DateTimeOffset? RequestToSpeakTimestamp => throw new NotImplementedException();

        IReadOnlyCollection<ClientType> IPresence.ActiveClients => throw new NotImplementedException();

        public Task AddRoleAsync(IRole role, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task AddRoleAsync(ulong roleId, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task AddRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task AddRolesAsync(IEnumerable<ulong> roleIds, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IDMChannel> CreateDMChannelAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
        {
            return "test url";
        }

        public string GetDefaultAvatarUrl()
        {
            throw new NotImplementedException();
        }

        public string GetDisplayAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
        {
            throw new NotImplementedException();
        }

        public string GetGuildAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
        {
            throw new NotImplementedException();
        }

        public Task<IDMChannel> GetOrCreateDMChannelAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public ChannelPermissions GetPermissions(IGuildChannel channel)
        {
            throw new NotImplementedException();
        }

        public Task KickAsync(string reason = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task ModifyAsync(Action<GuildUserProperties> func, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRoleAsync(IRole role, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRoleAsync(ulong roleId, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRolesAsync(IEnumerable<ulong> roleIds, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveTimeOutAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task SetTimeOutAsync(TimeSpan span, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }
    }
}
