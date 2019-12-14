using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;
using Watchman.DomainModel.Help.Queries;
using Watchman.Integrations.MongoDB;

namespace Watchman.Discord.Areas.Help.Controllers
{
    class HelpController
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly MessagesServiceFactory messagesServiceFactory;
        private readonly ISession _session;

        public HelpController(IQueryBus queryBus, ICommandBus commandBus, ISessionFactory sessionFactory, MessagesServiceFactory messagesServiceFactory)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this.messagesServiceFactory = messagesServiceFactory;
            this._session = sessionFactory.Create();
        }

        [DiscordCommand("-help")]
        public void PrintHelp(string message, Contexts contexts)
        {
            var result = this._queryBus.Execute(new GetHelpMessageQuery(this._session));
            var messagesService = messagesServiceFactory.Create(contexts);
            messagesService.SendMessage(result.HelpMessage);
        }
    }
}
