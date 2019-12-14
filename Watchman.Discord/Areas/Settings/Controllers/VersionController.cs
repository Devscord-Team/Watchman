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
        public void PrintVersion(string message, Contexts contexts)
        {
            var version = queryBus.Execute(new GetBotVersionQuery()).Version;

            var messagesService = new MessagesService { DefaultChannelId = contexts.Channel.Id };

            messagesService.SendMessage($"```Obecna wersja: {version}```");
        }
    }
}
