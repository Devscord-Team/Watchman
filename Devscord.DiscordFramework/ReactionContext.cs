using System;
using Discord;
using Discord.WebSocket;

namespace Devscord.DiscordFramework
{
    public class ReactionContext
    {
        public ISocketMessageChannel Channel { get; set; }
        public IEmote Emote { get; set; }
        public Optional<SocketUserMessage> Message { get; set; }
        public ulong MessageId { get; set; }
        public Optional<IUser> User { get; set; }
        public ulong UserId { get; set; }
        public DateTime SentAt { get; set; }
    }
}