namespace Watchman.DomainModel.Settings.ConfigurationItems
{
    public class MinAbsoluteMessagesCountToConsiderSafeUser : MappedConfiguration<int>
    {
        public override int Value { get; set; } = 300;

        public MinAbsoluteMessagesCountToConsiderSafeUser(ulong serverId) : base(serverId)
        {
        }
    }
}
