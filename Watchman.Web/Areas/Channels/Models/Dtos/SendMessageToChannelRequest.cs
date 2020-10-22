using TypeGen.Core.TypeAnnotations;

namespace Watchman.Web.Areas.Channels.Models.Dtos
{
    [ExportTsClass(OutputDir = "ClientApp/src/models")]
    public class SendMessageToChannelRequest
    {
        public string GuildId { private get; set; }
        public string ChannelId { private get; set; }
        public string Message { get; set; }

        public ulong GetGuildId => ulong.Parse(this.GuildId);
        public ulong GetChannelId => ulong.Parse(this.ChannelId);
    }
}
