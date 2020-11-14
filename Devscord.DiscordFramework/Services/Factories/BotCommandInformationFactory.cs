using Devscord.DiscordFramework.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using Autofac;
using System.Text.RegularExpressions;

namespace Devscord.DiscordFramework.Services.Factories
{
    public class BotCommandInformationFactory
    {
        private readonly Regex _areaNameRegex = new Regex(@"^Watchman\.Discord\.Areas\.(?<AreaName>\w+)", RegexOptions.Compiled | RegexOptions.Multiline);

        public BotCommandInformation Create(Type botCommand)
        {
            var properties = botCommand
                .GetProperties()
                .Where(prop => prop.CustomAttributes.Any(x => x.AttributeType.IsAssignableTo<CommandPropertyAttribute>()));

            var arguments = new List<BotArgumentInformation>();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof(CommandPropertyAttribute), inherit: true)
                    .Select(x => x as CommandPropertyAttribute)
                    .ToList();
                var expectedType = attributes.First(x => x.GetType() != typeof(Optional)).GetType();
                var isOptional = attributes.Any(x => x is Optional);

                var botArgumentInfo = new BotArgumentInformation(property.Name, expectedType, isOptional);
                arguments.Add(botArgumentInfo);
            }
            var areaName = this.GetAreaName(botCommand.Namespace);
            return new BotCommandInformation(botCommand.Name, areaName, arguments);
        }

        private string GetAreaName(string commandNamespace)
        {
            return this._areaNameRegex.Match(commandNamespace).Groups["AreaName"].Value;
        }
    }
}
