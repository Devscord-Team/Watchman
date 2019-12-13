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
using Devscord.DiscordFramework.Framework.Architecture.Controllers;

namespace Devscord.DiscordFramework.Services
{
    public class DbHelpGenerator : IService
    {
        private readonly ISession _session;
        private readonly Assembly _assembly;

        public DbHelpGenerator(ISessionFactory sessionFactory, Assembly botAssembly)
        {
            this._session = sessionFactory.Create();
            this._assembly = botAssembly;
        }

        public Task GenerateDefaultHelpDB()
        {
            CreateHelpInformation().ToList().ForEach(x => _session.Add(x));
            return Task.CompletedTask;
        }

        private IEnumerable<HelpInformation> CreateHelpInformation()
        {
            var helpInformation = new List<HelpInformation>();

            var contollers = _assembly.GetTypes()
                .Where(x => x.GetInterfaces().Any(i => i.FullName == typeof(IController).FullName));

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
