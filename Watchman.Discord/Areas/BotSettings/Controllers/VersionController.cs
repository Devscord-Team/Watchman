using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Watchman.Cqrs;

namespace Watchman.Discord.Areas.BotSettings.Controllers
{
    public class VersionController : IController
    {
        private readonly IQueryBus queryBus;
        private readonly ICommandBus commandBus;

        public VersionController(IQueryBus queryBus, ICommandBus commandBus)
        {
            this.queryBus = queryBus;
            this.commandBus = commandBus;
        }

        [DiscordCommand("-version")]
        public void PrintVersion(string message, Dictionary<string, IDiscordContext> contexts)
        {
            var version = File.ReadAllText("version.txt");
            var channel = (ChannelContext) contexts[nameof(ChannelContext)];
            var messagesService = new MessagesService() { DefaultChannelId = channel.Id };
            messagesService.SendMessage($"```Obecna wersja: {version}```");
        }
    }
}
