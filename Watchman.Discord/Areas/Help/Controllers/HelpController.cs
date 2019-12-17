using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
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
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly ISession _session;

        public HelpController(IQueryBus queryBus, ICommandBus commandBus, ISessionFactory sessionFactory, MessagesServiceFactory messagesServiceFactory)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._messagesServiceFactory = messagesServiceFactory;
            this._session = sessionFactory.Create();
        }

        [DiscordCommand("-help")]
        public void PrintHelp(string message, Contexts contexts)
        {
            var result = this._queryBus.Execute(new GetHelpInformationQuery(this._session, contexts.Server.Id));

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("```");

            foreach (var helpInfo in result.HelpInformations)
            {
                helpInfo.MethodNames.ToList().ForEach(x => messageBuilder.Append(x).Append(" / "));
                messageBuilder.Remove(messageBuilder.Length - 3, 3);
                
                messageBuilder.Append(" => ");

                messageBuilder.AppendLine(helpInfo.Descriptions.First(x => x.Name == helpInfo.DefaultDescriptionName).Details);
            }
            
            messageBuilder.AppendLine("```");

            var messagesService = _messagesServiceFactory.Create(contexts);
            messagesService.SendMessage(messageBuilder.ToString());
        }

        [DiscordCommand("-help json")]
        public void PrintJsonHelp(string message, Contexts contexts)
        {
            var result = this._queryBus.Execute(new GetHelpInformationQuery(this._session, contexts.Server.Id));

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("```");

            // todo: parse helpInfos to json
            foreach (var helpInfo in result.HelpInformations)
            {
                messageBuilder.AppendLine("{");

                messageBuilder.AppendLine($"\"commandId\" : \"{helpInfo.Id}\",");
                messageBuilder.AppendLine($"\"serverId\"");
                messageBuilder.AppendLine($"\"serverId\"");
                messageBuilder.AppendLine($"\"serverId\"");
                messageBuilder.AppendLine($"\"serverId\"");

                messageBuilder.AppendLine("},");
            }

            messageBuilder.Remove(messageBuilder.Length - 1, 1);
            messageBuilder.AppendLine("```");

            var messagesService = _messagesServiceFactory.Create(contexts);
            messagesService.SendMessage(messageBuilder.ToString());
        }
    }
}
