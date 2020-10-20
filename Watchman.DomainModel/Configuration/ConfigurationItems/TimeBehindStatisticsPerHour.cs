using System;

namespace Watchman.DomainModel.Settings.ConfigurationItems
{
    public class TimeBehindStatisticsPerHour : MappedConfiguration<TimeSpan>
    {
        public override TimeSpan Value { get; set; } = TimeSpan.FromHours(72);

        public TimeBehindStatisticsPerHour(ulong serverId) : base(serverId)
        {
        }
    }
}
