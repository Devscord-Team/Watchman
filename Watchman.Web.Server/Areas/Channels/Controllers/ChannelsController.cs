using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Web.Server.Areas.Channels.Models.Dtos;
using Watchman.Web.Server.Areas.Commons.Integration;

namespace Watchman.Web.Server.Areas.Channels.Controllers
{
    public class ChannelsController : BaseApiController
    {
        private readonly WatchmanService _watchmanService;

        public ChannelsController(WatchmanService watchmanService)
        {
            this._watchmanService = watchmanService;
        }

        [HttpPost]
        public async Task SendMessageToChannel([FromBody] SendMessageToChannelRequest request)
        {
            await this._watchmanService.SendMessageToChannel(request.GetGuildId, request.GetChannelId, request.Message);
        }
    }
}
