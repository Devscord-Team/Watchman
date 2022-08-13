using System;

namespace Watchman.DomainModel.Configuration.ConfigurationItems.TestItems
{
    public class TestNullableTimeItem : MappedConfiguration<TimeSpan?>
    {
        public override TimeSpan? Value { get; set; } = TimeSpan.FromDays(1);
        public override string Group { get; set; } = "TestItems";

        public TestNullableTimeItem(ulong serverId) : base(serverId)
        {
        }
    }
}