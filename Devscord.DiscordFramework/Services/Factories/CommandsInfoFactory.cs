using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Services.Factories
{
    public class CommandsInfoFactory
    {
        public IEnumerable<CommandInfo> Create(Type controller)
        {
            var methods = controller
                .GetMethods()
                .FilterMethodsByAttribute<DiscordCommand>();

            foreach (var methodInfo in methods)
            {
                var discordCommands = methodInfo.CustomAttributes.FilterAttributes<DiscordCommand>();
                var nameAttributes = discordCommands.Select(x => x.ConstructorArguments.First());
                var classFullName = methodInfo.ReflectedType?.FullName;

                yield return new CommandInfo
                {
                    Names = nameAttributes.Select(x => x.ToString().Replace("\"", "")),
                    MethodFullName = classFullName + '.' + methodInfo.Name,
                    CommandArgumentInfos = new List<CommandArgumentInfo> { new CommandArgumentInfo { Name = "Default" } }
                };
            }
        }
    }
}
