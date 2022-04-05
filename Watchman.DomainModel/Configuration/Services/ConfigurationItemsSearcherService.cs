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
            var results = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => !x.IsAbstract && x.IsClass && x.IsAssignableTo(typeof(IMappedConfiguration)))
                .ToList();
            return results;
        }
    }
}