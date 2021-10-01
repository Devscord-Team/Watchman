using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Discord.Areas.Configuration.BotCommands;
using Watchman.DomainModel.Configuration.Services;

namespace Watchman.Discord.Areas.Configuration.Controlles
{
    public class ConfigurationController : IController
    {
        private readonly IConfigurationService _configurationService;

        public ConfigurationController(IConfigurationService configurationService)
        {
            this._configurationService = configurationService;
        }

        [AdminCommand]
        public async Task SetConfig(SetConfgCommand command, Contexts contexts)
        {
            var configItem = this._configurationService.GetConfigurationItems(contexts.Server.Id)
                .FirstOrDefault(x => x.Name == command.Name);
            if(configItem == null)
            {
                throw new InvalidArgumentsException();
            }

            var configItemType = configItem.GetType().BaseType.GenericTypeArguments.First();

            var convertedType = Convert.ChangeType(command.Value, configItemType);

            configItem.GetType().GetProperty("Value").SetValue(configItem, convertedType);
            configItem.SetServerId(contexts.Server.Id);

            await this._configurationService.SaveNewConfiguration(configItem);
        }
        
    }
}
