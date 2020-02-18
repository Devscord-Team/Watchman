using System;

namespace Watchman.DomainModel.Users
{
    public class SpamDetectingStrategy
    {
        public ProtectionPunishment SelectPunishment(bool isAlerted, int messagesInLast10Seconds, int mutesInPast)
        {
            if (!isAlerted && messagesInLast10Seconds >= 5)
            {
                return new ProtectionPunishment(ProtectionPunishmentOption.Alert);
            }

            if (isAlerted && messagesInLast10Seconds >= 10)
            {
                if (mutesInPast > 4)
                {
                    return new ProtectionPunishment(ProtectionPunishmentOption.Ban);
                }

                return new ProtectionPunishment(ProtectionPunishmentOption.Mute,
                    TimeSpan.FromMinutes(15 * (mutesInPast + 1)));
            }

            if (isAlerted && messagesInLast10Seconds == 0)
            {
                return new ProtectionPunishment(ProtectionPunishmentOption.Clear);
            }

            return new ProtectionPunishment(ProtectionPunishmentOption.Nothing);
        }

    }
}