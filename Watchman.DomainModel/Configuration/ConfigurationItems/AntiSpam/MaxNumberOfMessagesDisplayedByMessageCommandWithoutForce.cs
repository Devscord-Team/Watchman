namespace Watchman.DomainModel.Configuration.ConfigurationItems.AntiSpam
{
    public class MaxNumberOfMessagesDisplayedByMessageCommandWithoutForce : MappedConfiguration<int>
    {
        public override int Value { get; set; } = 200;
        public override string Group { get; } = "AntiSpam";

        public MaxNumberOfMessagesDisplayedByMessageCommandWithoutForce(ulong serverId) : base(serverId)
        {
        }
    }
}
