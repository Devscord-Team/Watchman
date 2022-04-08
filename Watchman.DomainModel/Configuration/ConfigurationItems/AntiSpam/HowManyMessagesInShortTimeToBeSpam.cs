namespace Watchman.DomainModel.Configuration.ConfigurationItems.AntiSpam
{
    public class HowManyMessagesInShortTimeToBeSpam : MappedConfiguration<int>
    {
        public override int Value { get; set; } = 7;
        public override string Group { get; set; } = "AntiSpam";

        public HowManyMessagesInShortTimeToBeSpam(ulong serverId) : base(serverId)
        {
        }
    }
}
