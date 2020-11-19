using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Messages.Commands
{
    public class AddOrUpdatePreGeneratedStatisticsCommand : ICommand
    {
        public IEnumerable<PreGeneratedStatistic> PreGeneratedStatistics { get; }

        public AddOrUpdatePreGeneratedStatisticsCommand(IEnumerable<PreGeneratedStatistic> preGeneratedStatistics)
        {
            this.PreGeneratedStatistics = preGeneratedStatistics;
        }
    }
}
