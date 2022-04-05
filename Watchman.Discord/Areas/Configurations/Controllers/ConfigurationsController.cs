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
using Watchman.DomainModel.Configuration;
using Watchman.DomainModel.Configuration.Queries;
using Watchman.DomainModel.Configuration.Services;

namespace Watchman.Discord.Areas.Configurations.Controllers
{
    public class ConfigurationsController : IController
    {
        private readonly IMessagesServiceFactory messagesServiceFactory;
        private readonly IConfigurationService configurationService;

        public ConfigurationsController(IQueryBus queryBus, IMessagesServiceFactory messagesServiceFactory, IConfigurationService configurationService)
        {
            this.messagesServiceFactory = messagesServiceFactory;
            this.configurationService = configurationService;
        }

        //todo tests
        public async Task GetConfigurations(ConfigurationsCommand command, Contexts contexts)
        {
            var getConfigurationsQuery = new GetConfigurationItemsQuery(command.Group);
            var configurations = this.configurationService.GetConfigurationItems(contexts.Server.Id);
            if(command.Group != null)
            {
                configurations = configurations.Where(x => x.Group.ToLower() == command.Group.ToLower());
            }
            var groupped = configurations.GroupBy(x => x.Group).OrderBy(x => x.Key);
            var mapped = groupped.Select(x => new KeyValuePair<string, string>(x.Key, $"```\n{string.Join("\n", x.Select(item => item.Name))}```"));
            //todo from responses
            var messagesService = this.messagesServiceFactory.Create(contexts);
            await messagesService.SendEmbedMessage("Konfiguracja", "Poniżej znajdziesz liste elementów konfiguracji", mapped);
        }
    }
}
