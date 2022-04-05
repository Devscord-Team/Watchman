using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Watchman.DomainModel.Configuration.ConfigurationItems;

namespace Watchman.DomainModel.Configuration.Services
{
    public interface IConfigurationItemsSearcherService
    {
        List<Type> ConfigurationTypes { get; }
    }

    public class ConfigurationItemsSearcherService : IConfigurationItemsSearcherService
    {
        public List<Type> ConfigurationTypes { get; }

        public ConfigurationItemsSearcherService()
        {
            this.ConfigurationTypes = this.SearchForConfigurationTypes();
        }

        private List<Type> SearchForConfigurationTypes()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsClass && x.Namespace.StartsWith("Watchman.DomainModel.Configuration.ConfigurationItems"))
                .ToList();
        }
    }
}