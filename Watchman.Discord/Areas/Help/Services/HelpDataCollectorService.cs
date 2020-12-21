using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Services.Factories;
using Devscord.DiscordFramework.Services.Models;

namespace Watchman.Discord.Areas.Help.Services
{
    public class HelpDataCollectorService
    {
        private readonly BotCommandInformationFactory _botCommandInformationFactory;

        public HelpDataCollectorService(BotCommandInformationFactory botCommandInformationFactory)
        {
            this._botCommandInformationFactory = botCommandInformationFactory;
        }

        public IEnumerable<BotCommandInformation> GetBotCommandsInfo(Assembly botAssembly)
        {
            var botCommands = botAssembly.GetTypesByInterface<IBotCommand>();
            return botCommands.Select(x => this._botCommandInformationFactory.Create(x));
        }
    }
}
