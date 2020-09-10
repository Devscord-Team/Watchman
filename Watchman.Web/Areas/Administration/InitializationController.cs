using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Microsoft.AspNetCore.Mvc;
using Watchman.Web.Areas.Commons.Integration;

namespace Watchman.Web.Areas.Administration
{
    public class InitializationController :BaseApiController
    {
        private readonly WatchmanService _watchmanService;
        public InitializationController(WatchmanService watchmanService, IEnumerable<DiscordServerContext> contexts)
        {
            this._watchmanService = watchmanService;
        }
        
        [HttpGet]
        public async Task InitializeBotOnAllServers()
        {
            await this._watchmanService.InitializeAllServers();
        }
    }
}
