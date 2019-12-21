using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Services.Factories;
using Devscord.DiscordFramework.Services.Models;

namespace Devscord.DiscordFramework.Services
{
    public class HelpDataCollector : IService
    {
        private readonly IComponentContext _componentContext;

        public HelpDataCollector(IComponentContext componentContext)
        {
            this._componentContext = componentContext;
        }

        public IEnumerable<CommandInfo> GetCommandsInfo(Assembly botAssembly)
        {
            var controllers = botAssembly.GetTypesByInterface<IController>();
            var commandFactory = new CommandsInfoFactory();
            return controllers.SelectMany(x => commandFactory.Create(x));
        }
    }
}
