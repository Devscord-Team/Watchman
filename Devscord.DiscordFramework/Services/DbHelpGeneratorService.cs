using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Watchman.DomainModel.Help;
using Watchman.Integrations.MongoDB;

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
            _session = sessionFactory.Create();
        }

        public Task GenerateDefaultHelpDB(Assembly botAssembly)
        {
            CreateHelpInformation(botAssembly).ToList().ForEach(x => _session.Add(x));
            return Task.CompletedTask;
        }

        private IEnumerable<HelpInformation> CreateHelpInformation(Assembly botAssembly)
        {
            var helpInformation = new List<HelpInformation>();

            var controllers = GetControllers(botAssembly);

            var testHelp = new HelpInformation
            {
                ServerId = 0,
                MethodName = "testMethod"
            };

            var descriptions = new List<Description>
            {
                new Description {IsDefault = true, Details = "Empty", Name = "EN"}
            };

            testHelp.Descriptions = descriptions;

            helpInformation.Add(testHelp);

            return helpInformation;
        }

        private IEnumerable<IController> GetControllers(Assembly botAssembly)
        {
            var controllers = botAssembly.GetTypes()
                .Where(x => x.GetInterfaces().Any(i => i.FullName == typeof(IController).FullName))
                .Select(x => (IController) componentContext.Resolve(x));

            return controllers;
        }
    }
}