using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Services.Models;
using Serilog;
using Watchman.Discord.Areas.Help.Factories;
using Watchman.DomainModel.Help;
using Watchman.Integrations.MongoDB;

namespace Watchman.Discord.Areas.Help.Services
{
    public class HelpDBGeneratorService
    {
        private readonly HelpInformationFactory _helpInformationFactory;
        private readonly ISession _session;

        public HelpDBGeneratorService(ISessionFactory sessionFactory, HelpInformationFactory helpInformationFactory)
        {
            _helpInformationFactory = helpInformationFactory;
            _session = sessionFactory.Create();
        }

        public void FillDatabase(IEnumerable<CommandInfo> commandInfosFromAssembly)
        {
            var commandInfosFromAssemblyList = commandInfosFromAssembly.ToList(); // for not multiple enumerating
            var helpInfos = _session.Get<HelpInformation>().ToList();

            var newCommands = FindNewCommands(commandInfosFromAssemblyList, helpInfos).ToList();
            Task.Run(() => CheckIfExistsUselessHelp(commandInfosFromAssemblyList, helpInfos));

            var newHelpInfos = newCommands.Select(x => _helpInformationFactory.Create(x));
            _session.Add(newHelpInfos);
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
