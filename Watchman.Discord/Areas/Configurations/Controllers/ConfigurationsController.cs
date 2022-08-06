using Devscord.DiscordFramework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Configurations.BotCommands;
using Watchman.DomainModel.Configuration;
using Watchman.DomainModel.Configuration.Queries;
using Watchman.DomainModel.Configuration.Services;
using Watchman.Discord.ResponsesManagers;
using Devscord.DiscordFramework.Commons.Exceptions;
using Watchman.Discord.Areas.Configurations.Services;

namespace Watchman.Discord.Areas.Configurations.Controllers
{
    public class ConfigurationsController : IController
    {
        private readonly IMessagesServiceFactory _messagesServiceFactory;
        private readonly IConfigurationService _configurationService;
        private readonly IConfigurationMapperService _configurationMapperService;
        private readonly IConfigurationValueSetter _configurationValueSetter;

        public ConfigurationsController(IMessagesServiceFactory messagesServiceFactory, IConfigurationService configurationService, IConfigurationMapperService configurationMapperService, IConfigurationValueSetter configurationValueSetter)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._configurationService = configurationService;
            this._configurationMapperService = configurationMapperService;
            this._configurationValueSetter = configurationValueSetter;
        }

        //todo tests
        public async Task GetConfigurations(ConfigurationsCommand command, Contexts contexts)
        {
            var configurations = this._configurationService.GetConfigurationItems(contexts.Server.Id);
            if(command.Group != null)
            {
                configurations = configurations.Where(x => x.Group.ToLower() == command.Group.ToLower());
            }
            var groupped = configurations.GroupBy(x => x.Group).OrderBy(x => x.Key);
            var mapped = groupped.Select(x => new KeyValuePair<string, string>(x.Key, $"```\n{string.Join("\n", x.Select(item => item.Name))}```"));
            //todo from responses
            var messagesService = this._messagesServiceFactory.Create(contexts);
            await messagesService.SendEmbedMessage("Konfiguracja", "Poniżej znajdziesz liste elementów konfiguracji", mapped);
        }

        // TODO: Add tests
        public async Task SetCustomConfiguration(SetConfigurationCommand command, Contexts contexts)
        {
            var mappedConfiguration = this._configurationService.GetConfigurationItems(contexts.Server.Id)
                .FirstOrDefault(item => item.Name.ToLowerInvariant() == command.Name.ToLowerInvariant());
            var messageService = this._messagesServiceFactory.Create(contexts);

            if (mappedConfiguration == null)
            {
                await messageService.SendResponse(x => x.ConfigurationItemNotFound(command.Name));
                return;
            }

            var propertiesValues = command.GetType().GetProperties().Where(x => x.Name.EndsWith("Value")).Select(x => x.GetValue(command));
            var countOfPropertiesWithValue = propertiesValues.Count(x => x != null);
            if (countOfPropertiesWithValue > 1)
            {
                await messageService.SendResponse(x => x.TooManyValueArgumentsForSetConfiguration());
                return;
            }

            var configurationItem = this._configurationMapperService.MapIntoBaseFormat(mappedConfiguration, contexts.Server.Id);
            var configurationValueType = this._configurationService.GetConfigurationValueType(mappedConfiguration);
            if (countOfPropertiesWithValue == 0)
            {
                await this._configurationValueSetter.SetDefaultValueForConfiguration(configurationItem, configurationValueType);
                await messageService.SendResponse(x => x.ConfigurationValueHasBeenSetAsDefaultOfType(contexts, command.Name));
                return;
            }

            await this._configurationValueSetter.SetConfigurationValueFromCommand(command, configurationItem, propertiesValues, configurationValueType);
            await messageService.SendResponse(x => x.CustomConfigurationHasBeenSet(contexts, command.Name));
        }
    }
}
