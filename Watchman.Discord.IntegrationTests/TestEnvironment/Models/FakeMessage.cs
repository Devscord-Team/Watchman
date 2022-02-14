using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watchman.Discord.IntegrationTests.TestEnvironment.Models
{
    internal class FakeMessage : IMessage
    {
        public MessageType Type { get; set; }

        public MessageSource Source { get; set; }

        public bool IsTTS { get; set; }

        public bool IsPinned { get; set; }

        public bool IsSuppressed { get; set; }

        public string Content { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public DateTimeOffset? EditedTimestamp { get; set; }

        public IMessageChannel Channel { get; set; }

        public IUser Author { get; set; }

        public IReadOnlyCollection<IAttachment> Attachments { get; set; }

        public IReadOnlyCollection<IEmbed> Embeds { get; set; }

        public IReadOnlyCollection<ITag> Tags { get; set; }

        public IReadOnlyCollection<ulong> MentionedChannelIds { get; set; }

        public IReadOnlyCollection<ulong> MentionedRoleIds { get; set; }

        public IReadOnlyCollection<ulong> MentionedUserIds { get; set; }

        public MessageActivity Activity { get; set; }

        public MessageApplication Application { get; set; }

        public MessageReference Reference { get; set; }

        public IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public ulong Id { get; set; }

        public Task AddReactionAsync(IEmote emote, RequestOptions options = null)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(RequestOptions options = null)
        {
            return Task.CompletedTask;
        }

        public IAsyncEnumerable<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IEmote emoji, int limit, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAllReactionsAsync(RequestOptions options = null)
        {
            return Task.CompletedTask;
        }

        public Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions options = null)
        {
            return Task.CompletedTask;
        }

        public Task RemoveReactionAsync(IEmote emote, ulong userId, RequestOptions options = null)
        {
            return Task.CompletedTask;
        }
    }
}
