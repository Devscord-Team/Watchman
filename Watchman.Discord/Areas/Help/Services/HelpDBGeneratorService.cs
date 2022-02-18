﻿using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Services.Models;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Help.Factories;
using Watchman.DomainModel.Help;
using Watchman.DomainModel.Help.Commands;
using Watchman.DomainModel.Help.Queries;

namespace Watchman.Discord.Areas.Help.Services
{
    public interface IHelpDBGeneratorService
    {
        Task FillDatabase(IEnumerable<BotCommandInformation> commandInfosFromAssembly);
    }

    public class HelpDBGeneratorService : IHelpDBGeneratorService
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly IHelpInformationFactory _helpInformationFactory;

        public HelpDBGeneratorService(IQueryBus queryBus, ICommandBus commandBus, IHelpInformationFactory helpInformationFactory)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._helpInformationFactory = helpInformationFactory;
        }

        public Task FillDatabase(IEnumerable<BotCommandInformation> commandInfosFromAssembly)
        {
            var commandInfosFromAssemblyList = commandInfosFromAssembly.ToList();
            var query = new GetHelpInformationQuery(HelpInformation.EMPTY_SERVER_ID);
            var helpInfos = this._queryBus.Execute(query).HelpInformations.ToList();

            this.CheckIfExistsUselessHelp(commandInfosFromAssemblyList, helpInfos);

            var newCommands = this.FindNewCommands(commandInfosFromAssemblyList, helpInfos).ToList();
            if (!newCommands.Any())
            {
                return Task.CompletedTask;
            }
            var newHelpInfos = newCommands.Select(x => this._helpInformationFactory.Create(x));
            var command = new AddHelpInformationCommand(newHelpInfos);
            return this._commandBus.ExecuteAsync(command);
        }

        private void CheckIfExistsUselessHelp(IEnumerable<BotCommandInformation> commandInfosFromAssembly, IEnumerable<HelpInformation> helpInfos)
        {
            var oldUselessHelps = helpInfos.Where(h => commandInfosFromAssembly.All(c => c.Name != h.CommandName));
            foreach (var oldHelp in oldUselessHelps)
            {
                Log.Warning("Useless help info for method {oldHelp}", oldHelp.ToJson());
            }
        }

        private IEnumerable<BotCommandInformation> FindNewCommands(IEnumerable<BotCommandInformation> commandInfosFromAssembly, IEnumerable<HelpInformation> helpInfos)
        {
            var defaultHelpInfosInDb = helpInfos.Where(x => x.IsDefault); // for optimize checking only defaults
            return commandInfosFromAssembly.Where(x => defaultHelpInfosInDb.All(h => h.CommandName != x.Name));
        }
    }
}
