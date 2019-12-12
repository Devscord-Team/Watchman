using System;
using System.Collections.Generic;
using Devscord.DiscordFramework.Services;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using MongoDB.Driver;
using Watchman.Cqrs;
using Watchman.DomainModel.Help;
using Watchman.DomainModel.Help.Queries;
using Watchman.Integrations.MongoDB;

namespace Devscord.DiscordFramework.Services
{
    public class DbHelpGenerator : IService
    {
        private readonly IComponentContext _componentContext;
        private readonly ISession _session;

        public DbHelpGenerator(IComponentContext componentContext)
        {
            _componentContext = componentContext;
            

        }

        public Task GenerateDefaultHelpDB()
        {
            CreateHelpInformation().ToList().ForEach(x => _session.Add(x));
            return Task.CompletedTask;
        }

        private IEnumerable<HelpInformation> CreateHelpInformation()
        {
            var helpInformation = new List<HelpInformation>();

            var testHelp = new HelpInformation()
            {
                ServerId = 0,
                MethodName = "testMethod"
            };
            testHelp.Descriptions.Add(
                new Description()
                { IsDefault = true, Details = "Empty", Name = "EN" });
            return helpInformation;
        }
    }
}
