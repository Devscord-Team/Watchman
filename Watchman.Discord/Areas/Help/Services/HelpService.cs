using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Models;
using Watchman.Discord.Areas.Help.Factories;
using Watchman.DomainModel.Help.Models;
using Watchman.Integrations.MongoDB;

namespace Watchman.Discord.Areas.Help.Services
{
    class HelpService : IService
    {
        private readonly ISession _session;

        public HelpService(ISessionFactory sessionFactory)
        {
            _session = sessionFactory.Create();
        }

        public void FillDatabaseWithNewMethods(IEnumerable<CommandInfo> commandInfosFromAssembly)
        {
            var newCommands = FindNewCommands(commandInfosFromAssembly).ToList();
            var oldCommands = FindOldHelps(commandInfosFromAssembly).ToList();

            var helpInfoFactory = new HelpInformationFactory();
            newCommands.ForEach(x => _session.Add(helpInfoFactory.Create(x)));
            oldCommands.ForEach(x => _session.Delete(x));
        }

        private IEnumerable<CommandInfo> FindNewCommands(IEnumerable<CommandInfo> commandInfosFromAssembly)
        {
            var helpInfos = _session.Get<HelpInformation>().ToList();
            var defaultHelpInfosInDb = helpInfos.Where(x => x.IsDefault).ToList(); // for optimize checking only defaults
            return commandInfosFromAssembly.Where(x => defaultHelpInfosInDb.All(h => h.MethodName != x.MethodName));
        }

        private IEnumerable<HelpInformation> FindOldHelps(IEnumerable<CommandInfo> commandInfosFromAssembly)
        {
            var helpInfosInDb = _session.Get<HelpInformation>().ToList();
            return helpInfosInDb.Where(x => commandInfosFromAssembly.All(c => c.MethodName != x.MethodName));
        }
    }
}
