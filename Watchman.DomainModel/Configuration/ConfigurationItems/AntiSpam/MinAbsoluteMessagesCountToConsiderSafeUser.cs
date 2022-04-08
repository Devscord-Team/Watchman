namespace Watchman.DomainModel.Configuration.ConfigurationItems.AntiSpam
{
    public class MinAbsoluteMessagesCountToConsiderSafeUser : MappedConfiguration<int>
    {
        public override int Value { get; set; } = 300;
        public override string Group { get; set; } = "AntiSpam";

        public MinAbsoluteMessagesCountToConsiderSafeUser(ulong serverId) : base(serverId)
        {
        }
    }
}
