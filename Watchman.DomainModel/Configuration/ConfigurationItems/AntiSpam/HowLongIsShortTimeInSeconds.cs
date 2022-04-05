namespace Watchman.DomainModel.Configuration.ConfigurationItems.AntiSpam
{
    public class HowLongIsShortTimeInSeconds : MappedConfiguration<int>
    {
        public override int Value { get; set; } = 15;
        public override string Group { get; set; } = "AntiSpam";

        public HowLongIsShortTimeInSeconds(ulong serverId) : base(serverId)
        {
        }
    }
}
