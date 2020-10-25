using Devscord.DiscordFramework.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using Autofac;

namespace Devscord.DiscordFramework.Services.Factories
{
    public class BotCommandInformationFactory
    {
        public BotCommandInformation Create(Type botCommand)
        {
            var properties = botCommand
                .GetProperties()
                .Where(prop => prop.CustomAttributes.Any(x => x.AttributeType.IsAssignableTo<CommandPropertyAttribute>()));

            var arguments = new List<BotArgumentInformation>();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof(CommandPropertyAttribute), inherit: true).Select(x => x as CommandPropertyAttribute).ToList();
                var expectedType = attributes.First(x => x.GetType() != typeof(Optional)).GetType();
                var isOptional = attributes.Any(x => x is Optional);
                var botArgumentInfo = new BotArgumentInformation(property.Name, expectedType, isOptional);
                arguments.Add(botArgumentInfo);
            }
            return new BotCommandInformation(botCommand.Name, arguments);
        }
    }
}
