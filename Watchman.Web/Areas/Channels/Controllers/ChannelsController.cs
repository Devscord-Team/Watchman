using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Watchman.Web.Areas.Channels.Models.Dtos;
using Watchman.Web.Areas.Commons.Integration;

namespace Watchman.Web.Areas.Channels.Controllers
{
    public class ChannelsController : BaseApiController
    {
        private readonly WatchmanService _watchmanService;

        public ChannelsController(WatchmanService watchmanService) => this._watchmanService = watchmanService;

        [HttpPost]
        public async Task SendMessageToChannel([FromBody] SendMessageToChannelRequest request) => await this._watchmanService.SendMessageToChannel(request.GetGuildId, request.GetChannelId, request.Message);
    }
}
