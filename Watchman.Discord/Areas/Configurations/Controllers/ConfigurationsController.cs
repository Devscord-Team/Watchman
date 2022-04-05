using Devscord.DiscordFramework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Configurations.IBotCommands;
using Watchman.DomainModel.Configuration.Queries;

namespace Watchman.Discord.Areas.Configurations.Controllers
{
    public class ConfigurationsController : IController
    {
        private readonly IQueryBus queryBus;
        private readonly IMessagesServiceFactory messagesServiceFactory;

        public ConfigurationsController(IQueryBus queryBus, IMessagesServiceFactory messagesServiceFactory)
        {
            this.queryBus = queryBus;
            this.messagesServiceFactory = messagesServiceFactory;
        }

        public async Task GetConfigurations(ConfigurationsCommand command, Contexts contexts)
        {
            var getConfigurationsQuery = new GetConfigurationItemsQuery(command.Group);
            var configurationItems = (await this.queryBus.ExecuteAsync(getConfigurationsQuery)).ConfigurationItems;
            var messagesService = this.messagesServiceFactory.Create(contexts);
            var groupped = configurationItems.GroupBy(x => x.Group).OrderBy(x => x.Key);
            var mapped = groupped.Select(x => new KeyValuePair<string, string>(x.Key, $"```\n{string.Join("\n", x.Select(item => $"{item.Name} ({item.Value.GetType().Name})"))}```"));
            //todo from responses
            await messagesService.SendEmbedMessage("Konfiguracja", "Poniżej znajdziesz liste elementów konfiguracji", mapped);
        }
    }
}
