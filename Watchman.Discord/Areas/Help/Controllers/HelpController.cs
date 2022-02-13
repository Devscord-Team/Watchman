using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Help.BotCommands;
using System.Threading.Tasks;
using Watchman.Discord.Areas.Help.Services;
using Watchman.DomainModel.Help.Queries;
using Watchman.DomainModel.Help;
using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Commands.Responses;
using System;

namespace Watchman.Discord.Areas.Help.Controllers
{
    public class HelpController : IController
    {
        private readonly IHelpService helpService;

        public HelpController(IHelpService helpService)
        {
            this.helpService = helpService;
        }

        public Task PrintHelp(HelpCommand command, Contexts contexts)
        {
            var helpInformations = this.helpService.GetHelpInformations(contexts);
            return string.IsNullOrEmpty(command.Command)
                ? helpService.PrintHelpForAllCommands(command.Json, contexts, helpInformations)
                : this.helpService.PrintHelpForOneCommand(command.Command, contexts, helpInformations);
        }
    }
}
