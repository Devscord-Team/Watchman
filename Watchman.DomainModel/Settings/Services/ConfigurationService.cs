using System;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Settings.Commands;
using Watchman.DomainModel.Settings.Queries;

namespace Watchman.DomainModel.Settings.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public Configuration Configuration
        {
            get
            {
                if (this._lastRefreshed < DateTime.Now.AddMinutes(-10))
                {
                    this.ReloadConfiguration();
                }
                return this._cachedConfiguration;
            }
        }

        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private Configuration _cachedConfiguration;
        private DateTime _lastRefreshed;

        public ConfigurationService(IQueryBus queryBus, ICommandBus commandBus)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this.ReloadConfiguration();
        }

        public async Task SaveNewConfiguration()
        {
            var command = new ChangeConfigurationCommand(this._cachedConfiguration);
            await this._commandBus.ExecuteAsync(command);
        }

        public async Task SaveNewConfiguration(Configuration configuration)
        {
            var command = new ChangeConfigurationCommand(configuration);
            await this._commandBus.ExecuteAsync(command);
        }

        private void ReloadConfiguration()
        {
            var query = new GetConfigurationQuery();
            this._cachedConfiguration = this._queryBus.Execute(query).Configuration;
            this._lastRefreshed = DateTime.Now;
        }
    }
}
