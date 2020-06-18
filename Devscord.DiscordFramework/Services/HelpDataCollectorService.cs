using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Services.Factories;
using Devscord.DiscordFramework.Services.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Devscord.DiscordFramework.Services
{
    public class HelpDataCollectorService
    {
        private readonly CommandsInfoFactory _commandsInfoFactory;

        public HelpDataCollectorService(CommandsInfoFactory commandsInfoFactory) => this._commandsInfoFactory = commandsInfoFactory;

        public IEnumerable<CommandInfo> GetCommandsInfo(Assembly botAssembly)
        {
            var controllers = botAssembly.GetTypesByInterface<IController>();
            return controllers.SelectMany(x => this._commandsInfoFactory.Create(x));
        }
    }
}
