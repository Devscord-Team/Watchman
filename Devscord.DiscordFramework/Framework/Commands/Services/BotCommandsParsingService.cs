using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Devscord.DiscordFramework.Framework.Commands.Properties;
using Devscord.DiscordFramework.Commons.Exceptions;
using System.Collections.Generic;

namespace Devscord.DiscordFramework.Framework.Commands.Services
{
    public class BotCommandsParsingService
    {
        private readonly BotCommandsPropertyConversionService _botCommandPropertyConversionService;
        private readonly BotCommandsRequestValueGetterService _botCommandsRequestValueGetterService;

        public BotCommandsParsingService(BotCommandsPropertyConversionService botCommandPropertyConversionService, BotCommandsRequestValueGetterService botCommandsRequestValueGetterService)
        {
            this._botCommandPropertyConversionService = botCommandPropertyConversionService;
            this._botCommandsRequestValueGetterService = botCommandsRequestValueGetterService;
        }

        public IBotCommand ParseRequestToCommand(Type commandType, DiscordRequest request, BotCommandTemplate template)
        {
            return this.GetFilledInstance(commandType, template, (key, isList) => this._botCommandsRequestValueGetterService.GetValueByName(key, isList, request, template));
        }

        public IBotCommand ParseCustomTemplate(Type commandType, BotCommandTemplate template, Regex customTemplate, string input)
        {
            var match = customTemplate.Match(input);
            return this.GetFilledInstance(commandType, template, (key, isList) => this._botCommandsRequestValueGetterService.GetValueByNameFromCustomCommand(key, isList, template, match));
        }

        private IBotCommand GetFilledInstance(Type commandType, BotCommandTemplate template, Func<string, bool, object> getValueByName)
        {
            var instance = Activator.CreateInstance(commandType);
            foreach (var property in commandType.GetProperties())
            {
                var propertyType = template.Properties.First(x => x.Name == property.Name).Type;
                var isList = propertyType == BotCommandPropertyType.List;         
                var value = getValueByName.Invoke(property.Name, isList);
                if (string.IsNullOrWhiteSpace(value as string) && !isList)
                {
                    // if we got to this point in the code, it means that property is optional (but which is not bool) and the user did not provide a value to this field, so we assign null to property. Property must be a type that takes null.
                    property.SetValue(instance, null);
                }
                if (value is List<string> valueList)
                {
                    var IsEmptyList = string.IsNullOrWhiteSpace(valueList?.FirstOrDefault());
                    if (IsEmptyList)
                    {
                        property.SetValue(instance, null);
                        continue;
                    }
                    property.SetValue(instance, value);
                }
                if (value is string valueString)
                {
                    var convertedType = this._botCommandPropertyConversionService.ConvertType(valueString, propertyType);
                    property.SetValue(instance, convertedType);
                }
            }
            return (IBotCommand) instance;
        }
    }
}
