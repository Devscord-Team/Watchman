using System;

namespace Watchman.DomainModel.Configuration.ConfigurationItems
{
    public class TimeBehindStatisticsPerWeek : MappedConfiguration<TimeSpan>
    {
        public override TimeSpan Value { get; set; } = TimeSpan.FromDays(90);

        public TimeBehindStatisticsPerWeek(ulong serverId) : base(serverId)
        {
        }
    }
}
