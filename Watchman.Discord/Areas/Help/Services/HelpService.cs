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
            var helpInfosInDb = _session.Get<HelpInformation>().ToList();

            var newCommands = FindNewCommands(commandInfosFromAssembly, helpInfosInDb).ToList();
            var oldCommands = FindOldHelps(commandInfosFromAssembly, helpInfosInDb).ToList();
            var changedCommands = FindChangedHelps(commandInfosFromAssembly, helpInfosInDb).ToList();

            var helpInfoFactory = new HelpInformationFactory();
            newCommands.ForEach(x => _session.Add(helpInfoFactory.Create(x)));
            oldCommands.ForEach(x => _session.Delete(x));
            changedCommands.ForEach(x => _session.Update(UpdateHelpInfo(x, commandInfosFromAssembly)));
        }

        private IEnumerable<CommandInfo> FindNewCommands(IEnumerable<CommandInfo> commandInfosFromAssembly, IEnumerable<HelpInformation> helpInfosInDb)
        {
            var defaultHelpInfosInDb = helpInfosInDb.Where(x => x.IsDefault).ToList(); // for optimize checking only defaults
            return commandInfosFromAssembly.Where(x => defaultHelpInfosInDb.All(h => h.MethodName != x.MethodName));
        }

        private IEnumerable<HelpInformation> FindOldHelps(IEnumerable<CommandInfo> commandInfosFromAssembly, IEnumerable<HelpInformation> helpInfosInDb)
        {
            return helpInfosInDb.Where(x => commandInfosFromAssembly.All(c => c.MethodName != x.MethodName));
        }

        private IEnumerable<HelpInformation> FindChangedHelps(IEnumerable<CommandInfo> commandInfosFromAssembly, IEnumerable<HelpInformation> helpInfosInDb)
        {
            var allCommandsNamesInAssembly = commandInfosFromAssembly.SelectMany(x => x.Names);
            return helpInfosInDb.Where(x =>
            {
                return !x.Names.All(n => allCommandsNamesInAssembly.Contains(n))
                    || !allCommandsNamesInAssembly.All(n => x.Names.Contains(n));
            });
        }

        private HelpInformation UpdateHelpInfo(HelpInformation help, IEnumerable<CommandInfo> commandInfosFromAssembly)
        {
            help.Names = commandInfosFromAssembly.First(x => x.MethodName == help.MethodName).Names;
            return help;
        }
    }
}
