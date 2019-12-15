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
        private Description _defaultDescription => new Description
        {
            IsDefault = true,
            Name = "EN",
            Details = "Default text"
        };

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
            var controllers = GetControllers(botAssembly);
            var dbHelps = this._session.Get<HelpInformation>();

            var helpsInfo = controllers.SelectMany(this.MakeHelpInformations);
            
            foreach (var helpInfo in helpsInfo)
            {
                if (!dbHelps.Any(x => x.MethodNames.Equals(helpInfo.MethodNames)))
                    this._session.Add(helpInfo);
            }

            foreach (var dbHelpInfo in dbHelps)
            {
                if (!helpsInfo.Any(x => x.MethodNames.Equals(dbHelpInfo.MethodNames)))
                    this._session.Delete(dbHelpInfo);
            }

            return Task.CompletedTask;
        }

        private IEnumerable<IController> GetControllers(Assembly botAssembly)
        {
            var controllers = botAssembly.GetTypes()
                .Where(x => x.GetInterfaces().Any(i => i.FullName == typeof(IController).FullName))
                .Select(x => (IController)componentContext.Resolve(x));

            return controllers;
        }

        private IEnumerable<HelpInformation> MakeHelpInformations(IController controller)
        {
            var helpInformations = new List<HelpInformation>();
            var methods = controller.GetType().GetMethods()
                .Where(x => x.CustomAttributes
                    .Any(a => a.AttributeType.Name == nameof(DiscordCommand)));

            return methods.ToList().Select(x => new HelpInformation
            {
                Descriptions = new List<Description> { _defaultDescription },
                ServerId = 0,
                MethodNames = x.CustomAttributes.Where(x => x.AttributeType.Name == nameof(DiscordCommand))
                .Select(x => x.ConstructorArguments.First().ToString())
            });
        }
    }
}