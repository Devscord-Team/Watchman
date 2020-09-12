namespace Watchman.DomainModel.Configuration.ConfigurationItems
{
    public class MinUpperLettersCount : MappedConfiguration<int>
    {
        public override int Value { get; set; } = 8;

        public MinUpperLettersCount(ulong serverId) : base(serverId)
        {
        }
    }
}
