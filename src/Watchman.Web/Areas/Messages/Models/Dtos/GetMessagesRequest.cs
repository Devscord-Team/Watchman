namespace Watchman.Web.Areas.Messages.Models.Dtos
{
    public class GetMessagesRequest
    {
        public string GuildId { private get; set; }
        public string ChannelId { private get; set; }
        public string UserId { private get; set; }

        public ulong GetGuildId => ulong.Parse(this.GuildId);
        public ulong GetChannelId => ulong.Parse(this.ChannelId);
        public ulong? GetUserId => ulong.Parse(this.UserId);
    }
}
