using Discord;
using Discord.Audio;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Watchman.Discord.IntegrationTests.TestEnvironment.Models
{
    internal class FakeGuild : IGuild
    {
        public string Name { get; set; }

        public int AFKTimeout { get; set; }

        public bool IsEmbeddable { get; set; }

        public DefaultMessageNotifications DefaultMessageNotifications { get; set; }

        public MfaLevel MfaLevel { get; set; }

        public VerificationLevel VerificationLevel { get; set; }

        public ExplicitContentFilterLevel ExplicitContentFilter { get; set; }

        public string IconId { get; set; }

        public string IconUrl { get; set; }

        public string SplashId { get; set; }

        public string SplashUrl { get; set; }

        public bool Available { get; set; }

        public ulong? AFKChannelId { get; set; }

        public ulong DefaultChannelId { get; set; }

        public ulong? EmbedChannelId { get; set; }

        public ulong? SystemChannelId { get; set; }

        public ulong OwnerId { get; set; }

        public ulong? ApplicationId { get; set; }

        public string VoiceRegionId { get; set; }

        public IAudioClient AudioClient { get; set; }

        public IRole EveryoneRole { get; set; }

        public IReadOnlyCollection<GuildEmote> Emotes { get; set; }

        public IReadOnlyCollection<string> Features { get; set; }

        public IReadOnlyCollection<IRole> Roles { get; set; }

        public PremiumTier PremiumTier { get; set; }

        public string BannerId { get; set; }

        public string BannerUrl { get; set; }

        public string VanityURLCode { get; set; }

        public SystemChannelMessageDeny SystemChannelFlags { get; set; }

        public string Description { get; set; }

        public int PremiumSubscriptionCount { get; set; }

        public string PreferredLocale { get; set; }

        public CultureInfo PreferredCulture { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public ulong Id { get; set; }

        public bool IsWidgetEnabled => throw new NotImplementedException();

        public string DiscoverySplashId => throw new NotImplementedException();

        public string DiscoverySplashUrl => throw new NotImplementedException();

        public ulong? WidgetChannelId => throw new NotImplementedException();

        public ulong? RulesChannelId => throw new NotImplementedException();

        public ulong? PublicUpdatesChannelId => throw new NotImplementedException();

        public IReadOnlyCollection<ICustomSticker> Stickers => throw new NotImplementedException();

        public int? MaxPresences => throw new NotImplementedException();

        public int? MaxMembers => throw new NotImplementedException();

        public int? MaxVideoChannelUsers => throw new NotImplementedException();

        public int? ApproximateMemberCount => throw new NotImplementedException();

        public int? ApproximatePresenceCount => throw new NotImplementedException();

        public int MaxBitrate => throw new NotImplementedException();

        public NsfwLevel NsfwLevel => throw new NotImplementedException();

        public bool IsBoostProgressBarEnabled => throw new NotImplementedException();

        public ulong MaxUploadLimit => throw new NotImplementedException();

        GuildFeatures IGuild.Features => throw new NotImplementedException();

        public Task AddBanAsync(IUser user, int pruneDays = 0, string reason = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task AddBanAsync(ulong userId, int pruneDays = 0, string reason = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IGuildUser> AddGuildUserAsync(ulong userId, string accessToken, Action<AddGuildUserProperties> func = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IApplicationCommand>> BulkOverwriteApplicationCommandsAsync(ApplicationCommandProperties[] properties, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IApplicationCommand> CreateApplicationCommandAsync(ApplicationCommandProperties properties, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<ICategoryChannel> CreateCategoryAsync(string name, Action<GuildChannelProperties> func = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<GuildEmote> CreateEmoteAsync(string name, Image image, Optional<IEnumerable<IRole>> roles = default, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IGuildScheduledEvent> CreateEventAsync(string name, DateTimeOffset startTime, GuildScheduledEventType type, GuildScheduledEventPrivacyLevel privacyLevel = GuildScheduledEventPrivacyLevel.Private, string description = null, DateTimeOffset? endTime = null, ulong? channelId = null, string location = null, Image? coverImage = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IGuildIntegration> CreateIntegrationAsync(ulong id, string type, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IRole> CreateRoleAsync(string name, GuildPermissions? permissions = null, Color? color = null, bool isHoisted = false, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IRole> CreateRoleAsync(string name, GuildPermissions? permissions = null, Color? color = null, bool isHoisted = false, bool isMentionable = false, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IStageChannel> CreateStageChannelAsync(string name, Action<VoiceChannelProperties> func = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<ICustomSticker> CreateStickerAsync(string name, string description, IEnumerable<string> tags, Image image, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<ICustomSticker> CreateStickerAsync(string name, string description, IEnumerable<string> tags, string path, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<ICustomSticker> CreateStickerAsync(string name, string description, IEnumerable<string> tags, Stream stream, string filename, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<ITextChannel> CreateTextChannelAsync(string name, Action<TextChannelProperties> func = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IVoiceChannel> CreateVoiceChannelAsync(string name, Action<VoiceChannelProperties> func = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task DeleteEmoteAsync(GuildEmote emote, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task DeleteStickerAsync(ICustomSticker sticker, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task DisconnectAsync(IGuildUser user)
        {
            throw new NotImplementedException();
        }

        public Task DownloadUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IVoiceChannel> GetAFKChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IApplicationCommand> GetApplicationCommandAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IApplicationCommand>> GetApplicationCommandsAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IAuditLogEntry>> GetAuditLogsAsync(int limit = 100, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null, ulong? beforeId = null, ulong? userId = null, ActionType? actionType = null)
        {
            throw new NotImplementedException();
        }

        public Task<IBan> GetBanAsync(IUser user, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IBan> GetBanAsync(ulong userId, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IBan>> GetBansAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<ICategoryChannel>> GetCategoriesAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IGuildChannel> GetChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IGuildChannel>> GetChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IGuildUser> GetCurrentUserAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<ITextChannel> GetDefaultChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IGuildChannel> GetEmbedChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<GuildEmote> GetEmoteAsync(ulong id, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<GuildEmote>> GetEmotesAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IGuildScheduledEvent> GetEventAsync(ulong id, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IGuildScheduledEvent>> GetEventsAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IGuildIntegration>> GetIntegrationsAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IGuildUser> GetOwnerAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<ITextChannel> GetPublicUpdatesChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public IRole GetRole(ulong id)
        {
            throw new NotImplementedException();
        }

        public Task<ITextChannel> GetRulesChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IStageChannel> GetStageChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IStageChannel>> GetStageChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<ICustomSticker> GetStickerAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<ICustomSticker>> GetStickersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<ITextChannel> GetSystemChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            var channel = new FakeChannel() { Id = 5, Name = "TestSystemChannel" } as ITextChannel;
            return Task.FromResult(channel);
        }

        public Task<ITextChannel> GetTextChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<ITextChannel>> GetTextChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IThreadChannel> GetThreadChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IThreadChannel>> GetThreadChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IGuildUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IInviteMetadata> GetVanityInviteAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IVoiceChannel> GetVoiceChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IVoiceChannel>> GetVoiceChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IVoiceRegion>> GetVoiceRegionsAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IWebhook> GetWebhookAsync(ulong id, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IWebhook>> GetWebhooksAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IGuildChannel> GetWidgetChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task LeaveAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task ModifyAsync(Action<GuildProperties> func, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task ModifyEmbedAsync(Action<GuildEmbedProperties> func, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<GuildEmote> ModifyEmoteAsync(GuildEmote emote, Action<EmoteProperties> func, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task ModifyWidgetAsync(Action<GuildWidgetProperties> func, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task MoveAsync(IGuildUser user, IVoiceChannel targetChannel)
        {
            throw new NotImplementedException();
        }

        public Task<int> PruneUsersAsync(int days = 30, bool simulate = false, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<int> PruneUsersAsync(int days = 30, bool simulate = false, RequestOptions options = null, IEnumerable<ulong> includeRoleIds = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveBanAsync(IUser user, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveBanAsync(ulong userId, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task ReorderChannelsAsync(IEnumerable<ReorderChannelProperties> args, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task ReorderRolesAsync(IEnumerable<ReorderRoleProperties> args, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IGuildUser>> SearchUsersAsync(string query, int limit = 1000, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }
    }
}
