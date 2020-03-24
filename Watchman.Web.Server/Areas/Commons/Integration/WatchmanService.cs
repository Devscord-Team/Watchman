using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Discord.Integration.Channels.Commands;

namespace Watchman.Web.Server.Areas.Commons.Integration
{
    public class WatchmanService
    {
        private readonly ICommandBus _commandBus;

        public WatchmanService(ICommandBus commandBus)
        {
            this._commandBus = commandBus;
        }

        public async Task SendMessageToChannel(ulong channelId, string message)
        {
            var command = new SendMessageToChannelCommand(channelId, message);
            await this._commandBus.ExecuteAsync(command);
        }
    }
}
