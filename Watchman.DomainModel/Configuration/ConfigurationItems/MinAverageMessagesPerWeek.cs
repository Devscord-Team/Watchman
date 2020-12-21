namespace Watchman.DomainModel.Configuration.ConfigurationItems
{
    public class MinAverageMessagesPerWeek : MappedConfiguration<int>
    {
        public override int Value { get; set; } = 20;

        public MinAverageMessagesPerWeek(ulong serverId) : base(serverId)
        {
        }
    }
}
