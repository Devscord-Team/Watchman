using System;

namespace Watchman.DomainModel.Settings.ConfigurationItems
{
    public class TimeBehindStatisticsPerMinute : MappedConfiguration<TimeSpan>
    {
        public override TimeSpan Value { get; set; } = TimeSpan.FromMinutes(60);

        public TimeBehindStatisticsPerMinute(ulong serverId) : base(serverId)
        {
        }
    }
}
