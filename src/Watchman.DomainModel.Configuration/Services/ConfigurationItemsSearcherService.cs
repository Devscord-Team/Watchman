﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Watchman.DomainModel.Settings.ConfigurationItems;

namespace Watchman.DomainModel.Settings.Services
{
    public class ConfigurationItemsSearcherService
    {
        public List<Type> ConfigurationTypes { get; }

        public ConfigurationItemsSearcherService()
        {
            this.ConfigurationTypes = this.SearchForConfigurationTypes();
        }

        private List<Type> SearchForConfigurationTypes()
        {
            var configurationsNamespace = typeof(MinAverageMessagesPerWeek).Namespace;
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsClass && x.Namespace == configurationsNamespace)
                .ToList();
        }
    }
}