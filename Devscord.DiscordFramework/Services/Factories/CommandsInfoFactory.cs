using System;
using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Services.Models;

namespace Devscord.DiscordFramework.Services.Factories
{
    public class CommandsInfoFactory
    {
        public IEnumerable<CommandInfo> Create(Type controller)
        {
            var methods = controller.GetMethods();
            var discordCommands = methods.FilterMethodsByAttribute<DiscordCommand>();
            var commandsNotIgnored = discordCommands.Where(x => !x.CustomAttributes.Any(x => x.AttributeType == typeof(IgnoreForHelp)));

            return commandsNotIgnored.Select(x => new CommandInfo
            {
                Prefix = "-",
                Names = x.CustomAttributes.FilterAttributes<DiscordCommand>().Select(x => x.ConstructorArguments.First().ToString().Replace("\"", "")),
                MethodName = x.Name,
                CommandArgumentInfos = new List<CommandArgumentInfo>
                {
                    new CommandArgumentInfo
                    {
                        ArgumentPrefix = "",
                        Name = "Default"
                    }
                }
            });
        }
    }
}
