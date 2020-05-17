using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Services.Models;
using Serilog;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Help.Factories;
using Watchman.DomainModel.Help;
using Watchman.DomainModel.Help.Commands;
using Watchman.DomainModel.Help.Queries;

namespace Watchman.Discord.Areas.Help.Services
{
    public class HelpDBGeneratorService
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly HelpInformationFactory _helpInformationFactory;

        public HelpDBGeneratorService(IQueryBus queryBus, ICommandBus commandBus, HelpInformationFactory helpInformationFactory)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _helpInformationFactory = helpInformationFactory;
        }

        public async Task FillDatabase(IEnumerable<CommandInfo> commandInfosFromAssembly)
        {
            var commandInfosFromAssemblyList = commandInfosFromAssembly.ToList(); // for not multiple enumerating

            var query = new GetHelpInformationQuery(HelpInformation.DEFAULT_SERVER_INDEX);
            var helpInfos = _queryBus.Execute(query).HelpInformations.ToList();

            var newCommands = FindNewCommands(commandInfosFromAssemblyList, helpInfos).ToList();
            await Task.Run(() => CheckIfExistsUselessHelp(commandInfosFromAssemblyList, helpInfos));

            var newHelpInfos = newCommands.Select(x => _helpInformationFactory.Create(x));
            var command = new AddHelpInformationCommand(newHelpInfos);
            await _commandBus.ExecuteAsync(command);
        }

        private IEnumerable<CommandInfo> FindNewCommands(IEnumerable<CommandInfo> commandInfosFromAssembly, IEnumerable<HelpInformation> helpInfos)
        {
            var defaultHelpInfosInDb = helpInfos.Where(x => x.IsDefault).ToList(); // for optimize checking only defaults
            return commandInfosFromAssembly.Where(x => defaultHelpInfosInDb.All(h => h.MethodFullName != x.MethodFullName));
        }
        
        private Task CheckIfExistsUselessHelp(IEnumerable<CommandInfo> commandInfosFromAssembly, IEnumerable<HelpInformation> helpInfos)
        {
            var oldUselessHelps = helpInfos.Where(h => commandInfosFromAssembly.All(c => c.MethodFullName != h.MethodFullName));
            foreach (var oldHelp in oldUselessHelps)
            {
                Log.Warning($"Useless help info for method {oldHelp.MethodFullName}");
            }
            return Task.CompletedTask;
        }
    }
}
