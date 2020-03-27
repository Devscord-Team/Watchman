using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Watchman.Web.Server.Areas.Channels.Models.Dtos
{
    public class SendMessageToChannelRequest
    {
        public string GuildId { private get; set; }
        public string ChannelId { private get; set; }
        public string Message { get; set; }

        public ulong GetGuildId => ulong.Parse(GuildId);
        public ulong GetChannelId => ulong.Parse(ChannelId);
    }
}
