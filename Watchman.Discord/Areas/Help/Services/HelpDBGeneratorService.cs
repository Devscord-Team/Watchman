using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Models;
using Watchman.Discord.Areas.Help.Factories;
using Watchman.DomainModel.Help;
using Watchman.Integrations.MongoDB;

namespace Watchman.Discord.Areas.Help.Services
{
    public class HelpDBGeneratorService : IService
    {
        private readonly HelpInformationFactory _helpInformationFactory;
        private readonly ISession _session;

        public HelpDBGeneratorService(ISessionFactory sessionFactory, HelpInformationFactory helpInformationFactory)
        {
            _helpInformationFactory = helpInformationFactory;
            _session = sessionFactory.Create();
        }

        public void SyncDatabase(IEnumerable<CommandInfo> commandInfosFromAssembly)
        {
            var commandInfosFromAssemblyList = commandInfosFromAssembly.ToList(); // for not multiple enumerating
            var helpInfosInDb = _session.Get<HelpInformation>().ToList();

            var newCommands = FindNewCommands(commandInfosFromAssemblyList, helpInfosInDb).ToList();
            
            newCommands.ForEach(x => _session.Add(this._helpInformationFactory.Create(x)));
        }

        private IEnumerable<CommandInfo> FindNewCommands(IEnumerable<CommandInfo> commandInfosFromAssembly, IEnumerable<HelpInformation> helpInfosInDb)
        {
            var defaultHelpInfosInDb = helpInfosInDb.Where(x => x.IsDefault).ToList(); // for optimize checking only defaults
            return commandInfosFromAssembly.Where(x => defaultHelpInfosInDb.All(h => h.MethodName != x.MethodName));
        }
    }
}
