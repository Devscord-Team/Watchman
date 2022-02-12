using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Services.Factories;
using Devscord.DiscordFramework.Services.Models;

namespace Watchman.Discord.Areas.Help.Services
{
    public interface IHelpDataCollectorService
    {
        IEnumerable<BotCommandInformation> GetBotCommandsInfo(Assembly botAssembly);
    }

    public class HelpDataCollectorService : IHelpDataCollectorService
    {
        private readonly IBotCommandInformationFactory _botCommandInformationFactory;

        public HelpDataCollectorService(IBotCommandInformationFactory botCommandInformationFactory)
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
