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
    public class HelpService : IService
    {
        private Description _defaultDescription => new Description
        {
            Name = "EN",
            Details = "Default text"
        };

        private readonly ISession _session;
        private readonly IComponentContext _componentContext;

        public HelpService(IComponentContext componentContext)
        {
            this._componentContext = componentContext;
            var sessionFactory = componentContext.Resolve<ISessionFactory>();
            _session = sessionFactory.Create();
        }

        public Task GenerateDefaultHelpDB(Assembly botAssembly)
        {
            var controllers = GetControllers(botAssembly);
            var helpInformationInDb = this._session.Get<DefaultHelpInformation>();

            var generatedHelpInformation = controllers.SelectMany(this.CreateHelpInformation);

            AddNewDefaultHelpInformation(helpInformationInDb, generatedHelpInformation);
            RemoveOldDefaultHelpInformation(helpInformationInDb, generatedHelpInformation);

            var serverHelpInformationInDb = this._session.Get<ServerHelpInformation>();
            RemoveOldDefaultHelpInformation(serverHelpInformationInDb, generatedHelpInformation);

            return Task.CompletedTask;
        }

        private IEnumerable<IController> GetControllers(Assembly botAssembly)
        {
            var controllers = botAssembly.GetTypes()
                .Where(x => x.GetInterfaces().Any(i => i.FullName == typeof(IController).FullName))
                .Select(x => (IController)_componentContext.Resolve(x));

            return controllers;
        }

        private IEnumerable<DefaultHelpInformation> CreateHelpInformation(IController controller)
        {
            var methods = controller.GetType().GetMethods()
                .Where(x => x.CustomAttributes
                    .Any(a => a.AttributeType.Name == nameof(DiscordCommand)));

            return methods.ToList().Select(x => new DefaultHelpInformation
            {
                Descriptions = new List<Description> { _defaultDescription },
                DefaultDescriptionName = "EN",
                Name = x.Name,
                MethodNames = x.CustomAttributes
                    .Where(x => x.AttributeType.Name == nameof(DiscordCommand))
                    .Select(x => x.ConstructorArguments.First().ToString().Replace("\"", ""))
            });
        }

        private void AddNewDefaultHelpInformation(IQueryable<DefaultHelpInformation> helpInformationInDb, IEnumerable<DefaultHelpInformation> generatedHelpInformation)
        {
            foreach (var generatedHelpInfo in generatedHelpInformation)
            {
                if (!helpInformationInDb.Any(x => x.MethodNames.Equals(generatedHelpInfo.MethodNames)))
                    this._session.Add(generatedHelpInfo);
            }
        }

        private void RemoveOldDefaultHelpInformation<T>(IQueryable<T> helpInformationInDb, IEnumerable<DefaultHelpInformation> generatedHelpInformation) where T : DefaultHelpInformation
        {
            foreach (var dbHelpInfo in helpInformationInDb)
            {
                if (!generatedHelpInformation.Any(x => x.MethodNames.SequenceEqual(dbHelpInfo.MethodNames)))
                    this._session.Delete(dbHelpInfo);
            }
        }
    }
}