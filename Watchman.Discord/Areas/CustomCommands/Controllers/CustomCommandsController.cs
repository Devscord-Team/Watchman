using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Discord.Areas.CustomCommands.BotCommands;
using Watchman.Discord.Areas.Users.BotCommands;
using Watchman.DomainModel.CustomCommands.Queries;

namespace Watchman.Discord.Areas.CustomCommands.Controllers
{
    public class CustomCommandsController : IController
    {
        private readonly IQueryBus queryBus;
        private readonly MessagesServiceFactory messagesServiceFactory;

        public CustomCommandsController(IQueryBus queryBus, MessagesServiceFactory messagesServiceFactory)
        {
            this.queryBus = queryBus;
            this.messagesServiceFactory = messagesServiceFactory;
        }

        public async Task PrintCustomCommands(CustomCommandsCommand command, Contexts contexts)
        {
            var getCustomCommandsQuery = new GetCustomCommandsQuery(contexts.Server.Id);
            var customCommands = await this.queryBus.ExecuteAsync(getCustomCommandsQuery);
            var messagesServices = this.messagesServiceFactory.Create(contexts);
            await messagesServices.SendEmbedMessage("Custom commands", string.Empty, customCommands.CustomCommands.Select(x => new KeyValuePair<string, string>(x.CommandFullName, x.CustomTemplateRegex)));
        }
    }
}
