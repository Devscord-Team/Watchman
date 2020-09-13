using System.Threading.Tasks;
using Autofac;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Microsoft.Extensions.Hosting;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Initialization.Services;
using Watchman.Discord.Integration.Channels.Commands;

namespace Watchman.Web.Areas.Commons.Integration
{
    public class WatchmanService
    {
        private readonly ICommandBus _commandBus;
        
        public WatchmanService(ICommandBus commandBus)
        {
            this._commandBus = commandBus;           
        }

        public async Task SendMessageToChannel(ulong guildId, ulong channelId, string message)
        {
            var command = new SendMessageToChannelCommand(guildId, channelId, message);
            await this._commandBus.ExecuteAsync(command);
        }
    }
}
