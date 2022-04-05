namespace Watchman.DomainModel.Configuration.ConfigurationItems.AntiSpam
{
    public class MinUpperLettersCount : MappedConfiguration<int>
    {
        public override int Value { get; set; } = 8;
        public override string Group { get; } = "AntiSpam";

        public MinUpperLettersCount(ulong serverId) : base(serverId)
        {
        }
    }
}
