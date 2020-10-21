﻿namespace Watchman.DomainModel.Configuration.ConfigurationItems
{
    public class HowManyMessagesInShortTimeToBeSpam : MappedConfiguration<int>
    {
        public override int Value { get; set; } = 7;

        public HowManyMessagesInShortTimeToBeSpam(ulong serverId) : base(serverId)
        {
        }
    }
}
