namespace Watchman.DomainModel.Configuration.ConfigurationItems.AntiSpam
{
    public class MinAverageMessagesPerWeek : MappedConfiguration<int>
    {
        public override int Value { get; set; } = 20;
        public override string Group { get; } = "AntiSpam";

        public MinAverageMessagesPerWeek(ulong serverId) : base(serverId)
        {
        }
    }
}
