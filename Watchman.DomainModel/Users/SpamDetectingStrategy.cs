namespace Watchman.DomainModel.Users
{
    public class SpamDetectingStrategy
    {
        public ProtectionPunishment SelectPunishment(int warnsInLastFewMinutes, int warnsInLastFewHours, int mutesInLastFewHours, int messagesInLastFewSeconds, int messagesInLastFewMinutes)
        {
            if (messagesInLastFewSeconds >= 5 && messagesInLastFewMinutes >= 40)
            {
                if (mutesInLastFewHours > 0 && warnsInLastFewMinutes > 2)
                {
                    return new ProtectionPunishment(ProtectionPunishmentOption.LongMute);
                }

                if (warnsInLastFewMinutes > 1 && warnsInLastFewHours > 3)
                {
                    return new ProtectionPunishment(ProtectionPunishmentOption.Mute);
                }

                return new ProtectionPunishment(ProtectionPunishmentOption.Warn);
            }

            return new ProtectionPunishment(ProtectionPunishmentOption.Nothing);
        }
    }
}