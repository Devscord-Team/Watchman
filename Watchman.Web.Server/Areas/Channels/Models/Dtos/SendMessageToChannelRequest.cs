using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Watchman.Web.Server.Areas.Channels.Models.Dtos
{
    public class SendMessageToChannelRequest
    {
        public ulong ChannelId { get; set; }
        public string Message { get; set; }
    }
}
