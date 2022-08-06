using Devscord.DiscordFramework.Commons.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Discord.Areas.Configurations.BotCommands;
using Watchman.DomainModel.Configuration;
using Watchman.DomainModel.Configuration.Services;

namespace Watchman.Discord.Areas.Configurations.Services
{
    public interface IConfigurationValueSetter
    {
        Task SetDefaultValueForConfiguration(ConfigurationItem configurationItem, Type valueType);
        Task SetConfigurationValueFromCommand(SetConfigurationCommand command, ConfigurationItem configurationItem, IEnumerable<object> propertiesValues, Type configurationValueType);
    }

    // TODO: Add tests
    public class ConfigurationValueSetter : IConfigurationValueSetter
    {
        private readonly IConfigurationService _configurationService;

        public ConfigurationValueSetter(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task SetDefaultValueForConfiguration(ConfigurationItem configurationItem, Type valueType)
        {
            var defaultTypeValue = valueType.IsValueType ? Activator.CreateInstance(valueType) : null;
            await this.SetNewConfiguration(configurationItem, defaultTypeValue);
        }

        public async Task SetConfigurationValueFromCommand(SetConfigurationCommand command, ConfigurationItem configurationItem, IEnumerable<object> propertiesValues, Type configurationValueType)
        {
            var valueToSet = command switch
            {
                SetConfigurationCommand com when com.NumberValue != null 
                    => this.GetNumberValue(command.NumberValue, configurationValueType),

                SetConfigurationCommand com when com.BoolValue != null 
                    => this.GetBoolValue(command.BoolValue, configurationValueType),

                _ => this.GetCustomValue(propertiesValues, configurationValueType)
            };
            await this.SetNewConfiguration(configurationItem, valueToSet);
        }

        private object GetNumberValue(double? numberValue, Type configurationValueType)
        {
            var underlyingValueType = this.GetUnderlyingType(configurationValueType);
            var doesItemAcceptNumbers = underlyingValueType.IsValueType 
                ? Activator.CreateInstance(underlyingValueType).ToString() == "0"
                : false;

            if (!doesItemAcceptNumbers)
            {
                throw new InvalidArgumentsException();
            }
            return Convert.ChangeType(numberValue, configurationValueType);
        }

        private bool GetBoolValue(string valueInText, Type configurationValueType)
        {
            var isValueConvertibleToBool = bool.TryParse(valueInText, out var convertedValue);
            var doesItemAcceptBool = configurationValueType == typeof(bool) || configurationValueType == typeof(bool?);

            if (!isValueConvertibleToBool || !doesItemAcceptBool)
            {
                throw new InvalidArgumentsException();
            }
            return convertedValue;
        }

        private object GetCustomValue(IEnumerable<object> propertiesValues, Type configurationValueType)
        {
            var providedValue = propertiesValues.First(x => x != null);

            var underlyingTypeOfProvidedValue = this.GetUnderlyingType(providedValue.GetType());
            var underlyingConfigurationType = this.GetUnderlyingType(configurationValueType);

            if (underlyingTypeOfProvidedValue != underlyingConfigurationType)
            {
                throw new InvalidArgumentsException();
            }
            return providedValue;
        }

        private Type GetUnderlyingType(Type type)
            => Nullable.GetUnderlyingType(type) ?? type;

        private async Task SetNewConfiguration(ConfigurationItem configurationItem, object valueToSet)
        {
            configurationItem.SetValue(valueToSet);
            await this._configurationService.SaveNewConfiguration(configurationItem);
        }
    }
}
