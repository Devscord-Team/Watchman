namespace Watchman.DomainModel.Configuration.ConfigurationItems
{
    public class MaxNumberOfMessagesDisplayedByMessageCommandWithoutForce : MappedConfiguration<int>
    {
        public override int Value { get; set; } = 200;

        public MaxNumberOfMessagesDisplayedByMessageCommandWithoutForce(ulong serverId) : base(serverId)
        {
        }
    }
}
