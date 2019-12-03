using System;

namespace Watchman.DomainModel.Protection
{
    public class ProtectionPunishment
    {
        public ProtectionPunishmentOptions Option { get; private set; }
        public TimeSpan? Time { get; private set; }

        public ProtectionPunishment(ProtectionPunishmentOptions option, TimeSpan? time = null)
        {
            Option = option;
            Time = time;
        }
    }
}
