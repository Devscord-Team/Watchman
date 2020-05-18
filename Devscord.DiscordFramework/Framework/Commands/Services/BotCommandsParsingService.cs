using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using System;
using System.Linq;

namespace Devscord.DiscordFramework.Framework.Commands.Services
{
    public class BotCommandsParsingService
    {
        private readonly BotCommandsPropertyConversionService botCommandPropertyConversionService;

        public BotCommandsParsingService(BotCommandsPropertyConversionService botCommandPropertyConversionService)
        {
            this.botCommandPropertyConversionService = botCommandPropertyConversionService;
        }

        public IBotCommand ParseRequestToCommand(Type commandType, DiscordRequest request, BotCommandTemplate template)
        {
            var instance = Activator.CreateInstance(commandType);
            foreach (var property in commandType.GetProperties())
            {
                var value = request.Arguments.FirstOrDefault(x => x.Name.ToLowerInvariant() == property.Name.ToLowerInvariant())?.Value;
                if (value == null)
                {
                    continue;
                }
                var propertyType = template.Properties.First(x => x.Name == property.Name).Type;
                var convertedType = botCommandPropertyConversionService.ConvertType(value, propertyType);
                property.SetValue(instance, convertedType);
            }
            return (IBotCommand)instance;
        }
    }
}
