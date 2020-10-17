using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.DomainModel.Settings.ConfigurationItems
{
    public class TimeBehindStatisticsPerDay : MappedConfiguration<TimeSpan>
    {
        public override TimeSpan Value { get; set; } = TimeSpan.FromDays(30);

        public TimeBehindStatisticsPerDay(ulong serverId) : base(serverId)
        {
        }
    }
}
