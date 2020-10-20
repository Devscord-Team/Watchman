using System;

namespace Watchman.DomainModel.Settings.ConfigurationItems
{
    public class TimeBehindStatisticsPerWeek : MappedConfiguration<TimeSpan>
    {
        public override TimeSpan Value { get; set; } = TimeSpan.FromDays(90);

        public TimeBehindStatisticsPerWeek(ulong serverId) : base(serverId)
        {
        }
    }
}
