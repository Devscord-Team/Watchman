﻿using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Cqrs;
using Watchman.DomainModel.Settings.Queries;

namespace Watchman.Discord.Areas.Settings.Controllers
{
    public class VersionController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly MessagesServiceFactory _messagesServiceFactory;

        public VersionController(IQueryBus queryBus, MessagesServiceFactory messagesServiceFactory)
        {
            this._queryBus = queryBus;
            this._messagesServiceFactory = messagesServiceFactory;
        }

        [AdminCommand]
        [DiscordCommand("version")]
        [IgnoreForHelp]
        public void PrintVersion(DiscordRequest request, Contexts contexts)
        {
            var version = _queryBus.Execute(new GetBotVersionQuery()).Version;
            var messagesService = _messagesServiceFactory.Create(contexts);
            messagesService.SendResponse(x => x.CurrentVersion(contexts, version), contexts);
        }
    }
}
