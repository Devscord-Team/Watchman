using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Watchman.Web.Server.Areas.Messages.Models.Dtos
{
    public class GetMessagesRequest
    {
        public string GuildId { private get; set; }
        public string ChannelId { private get; set; }
        public string UserId { private get; set; }

        public ulong GetGuildId => ulong.Parse(GuildId);
        public ulong GetChannelId => ulong.Parse(ChannelId);
        public ulong? GetUserId => ulong.Parse(UserId);
    }
}
