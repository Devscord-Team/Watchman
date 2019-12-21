﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Services.Models;

namespace Devscord.DiscordFramework.Services
{
    public class HelpDataCollector : IService
    {
        private readonly IComponentContext _componentContext;

        public HelpDataCollector(IComponentContext componentContext)
        {
            this._componentContext = componentContext;
        }

        public IEnumerable<CommandInfo> GenerateDefaultHelpDbCollection(Assembly botAssembly)
        {
            var controllers = botAssembly.GetTypesByInterface<IController>();

            // get controllers
            // make commandInfo from them
            // return the list

            var helpInformationInDb = this._session.Get<DefaultHelpInformation>().ToList();

            var generatedHelpInformation = controllers.SelectMany(this.CreateHelpInformation).ToList();

            AddNewDefaultHelpInformation(helpInformationInDb, generatedHelpInformation);
            RemoveOldDefaultHelpInformation(helpInformationInDb, generatedHelpInformation);

            var serverHelpInformationInDb = this._session.Get<DefaultHelpInformation>();
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

        private void AddNewDefaultHelpInformation(IEnumerable<DefaultHelpInformation> helpInformationInDb, IEnumerable<DefaultHelpInformation> generatedHelpInformation)
        {
            foreach (var generatedHelpInfo in generatedHelpInformation)
            {
                if (!helpInformationInDb.Any(x => x.MethodNames.Equals(generatedHelpInfo.MethodNames)))
                    this._session.Add(generatedHelpInfo);
            }
        }

        private void RemoveOldDefaultHelpInformation<T>(IEnumerable<T> helpInformationInDb, IEnumerable<DefaultHelpInformation> generatedHelpInformation) where T : DefaultHelpInformation
        {
            foreach (var dbHelpInfo in helpInformationInDb)
            {
                if (!generatedHelpInformation.Any(x => x.MethodNames.SequenceEqual(dbHelpInfo.MethodNames)))
                    this._session.Delete(dbHelpInfo);
            }
        }
    }
}