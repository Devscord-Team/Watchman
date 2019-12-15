using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Cqrs;
using Watchman.DomainModel.Help.Queries;
using Watchman.Integrations.MongoDB;

namespace Watchman.Discord.Areas.Help.Controllers
{
    class HelpController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly ISession _session;

        public HelpController(IQueryBus queryBus, ICommandBus commandBus, ISessionFactory sessionFactory)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._session = sessionFactory.Create();
        }

        [DiscordCommand("-help")]
        public void PrintHelp(string message, Dictionary<string, IDiscordContext> contexts)
        {
            var result = this._queryBus.Execute(new GetHelpInformationQuery(this._session));
            var channelContext = (ChannelContext)contexts[nameof(ChannelContext)];

            var messagesService = new MessagesService { DefaultChannelId = channelContext.Id };

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("```");

            foreach (var helpInfo in result.HelpInformations)
            {
                helpInfo.MethodNames.ToList().ForEach(x => messageBuilder.Append(x).Append(" / "));
                messageBuilder.Remove(messageBuilder.Length - 3, 3);
            }

            messageBuilder.AppendLine("```");
            messagesService.SendMessage(messageBuilder.ToString());
        }
    }
}
