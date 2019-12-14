using System.Collections.Generic;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Watchman.Cqrs;
using Watchman.DomainModel.Settings.Queries;

namespace Watchman.Discord.Areas.Settings.Controllers
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

        [AdminCommand]
        [DiscordCommand("-version")]
        public void PrintVersion(string message, Dictionary<string, IDiscordContext> contexts)
        {
            var version = queryBus.Execute(new GetBotVersionQuery()).Version;

            var channel = (ChannelContext) contexts[nameof(ChannelContext)];
            var messagesService = new MessagesService { DefaultChannelId = channel.Id };

            messagesService.SendMessage($"```Obecna wersja: {version}```");
        }
    }
}
