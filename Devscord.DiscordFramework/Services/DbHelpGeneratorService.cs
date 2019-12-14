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
    public class DbHelpGeneratorService : IService
    {
        private readonly ISession _session;
        private readonly IComponentContext componentContext;

        public DbHelpGeneratorService(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
            var sessionFactory = componentContext.Resolve<ISessionFactory>();
            this._session = sessionFactory.Create();
        }

        public Task GenerateDefaultHelpDB()
        {
            CreateHelpInformation().ToList().ForEach(x => _session.Add(x));
            return Task.CompletedTask;
        }

        private IEnumerable<HelpInformation> CreateHelpInformation()
        {
            var helpInformation = new List<HelpInformation>();

            var controllers = GetControllers();

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

        private IEnumerable<IController> GetControllers()
        {
            var contollers = typeof(Workflow).Assembly.GetTypes()
                .Where(x => x.GetInterfaces().Any(i => i.FullName == typeof(IController).FullName))
                .Select(x => (IController)componentContext.Resolve(x));

            return contollers;
        }
    }
}
