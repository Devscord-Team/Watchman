using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Services.Factories;
using Devscord.DiscordFramework.Services.Models;

namespace Devscord.DiscordFramework.Services
{
    public class HelpDataCollector : IService
    {
        private readonly CommandsInfoFactory _commandsInfoFactory;

        public HelpDataCollector()
        {
            this._commandsInfoFactory = new CommandsInfoFactory();
        }

        public IEnumerable<CommandInfo> GetCommandsInfo(Assembly botAssembly)
        {
            var controllers = botAssembly.GetTypesByInterface<IController>();
            return controllers.SelectMany(x => _commandsInfoFactory.Create(x));
        }
    }
}
