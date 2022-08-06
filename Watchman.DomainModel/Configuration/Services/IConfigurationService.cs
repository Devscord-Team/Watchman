﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Watchman.DomainModel.Configuration.Services
{
    public interface IConfigurationService
    {
        T GetConfigurationItem<T>(ulong serverId) where T : IMappedConfiguration;
        IEnumerable<IMappedConfiguration> GetConfigurationItems(ulong serverId);
        Task SaveNewConfiguration(ConfigurationItem changedConfiguration);
        Task InitDefaultConfigurations();
        Type GetConfigurationValueType(IMappedConfiguration mappedConfiguration);
    }
}
