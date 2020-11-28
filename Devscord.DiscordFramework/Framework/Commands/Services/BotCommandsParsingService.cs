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
                var botCommandProperty = template.Properties.First(x => x.Name == property.Name);
                var isList = botCommandProperty.GeneralType == BotCommandPropertyType.List;         
                var value = getValueByName.Invoke(property.Name, isList);
                if (string.IsNullOrWhiteSpace(value as string) && !isList)
                {
                    // if user didn't provide a value to the optional (but not bool) property, we assign null, so an optional property has to be nullable
                    property.SetValue(instance, null);
                    continue;
                }
                if (value is List<string> valueList)
                {
                    var isEmptyList = string.IsNullOrWhiteSpace(valueList?.FirstOrDefault());
                    if (isEmptyList)
                    {
                        property.SetValue(instance, null);
                        continue;
                    }
                    property.SetValue(instance, value);
                }
                if (value is string valueString)
                {
                    var convertedType = this._botCommandPropertyConversionService.ConvertType(valueString, botCommandProperty);
                    property.SetValue(instance, convertedType);
                }
            }
            return (IBotCommand) instance;
        }
    }
}
