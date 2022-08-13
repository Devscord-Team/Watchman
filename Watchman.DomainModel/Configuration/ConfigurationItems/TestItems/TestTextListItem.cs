using System.Collections.Generic;

namespace Watchman.DomainModel.Configuration.ConfigurationItems.TestItems
{
    public class TestTextListItem : MappedConfiguration<List<string>>
    {
        public override List<string> Value { get; set; } = null;
        public override string Group { get; set; } = "TestItems";

        public TestTextListItem(ulong serverId) : base(serverId)
        {
        }
    }
}