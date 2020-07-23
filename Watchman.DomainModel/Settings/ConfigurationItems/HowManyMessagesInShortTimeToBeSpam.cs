namespace Watchman.DomainModel.Settings.ConfigurationItems
{
    public class HowManyMessagesInShortTimeToBeSpam : MappedConfiguration<int>
    {
        public override int Value { get; set; } = 15;

        public HowManyMessagesInShortTimeToBeSpam(ulong serverId) : base(serverId)
        {
        }
    }
}
