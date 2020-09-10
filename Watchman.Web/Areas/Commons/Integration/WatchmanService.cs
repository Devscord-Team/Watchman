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
        //TODO: improve this method, I had no idea how to get DiscordServerContext so I left it like that - looking forward to your help
        public async Task InitializeAllServers()
        {
            Program.CreateHostBuilder(new string[] { }, Program.GetConfiguration()).Build().Run();
        }
    }
}
