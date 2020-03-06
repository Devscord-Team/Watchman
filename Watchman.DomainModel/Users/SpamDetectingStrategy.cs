namespace Watchman.DomainModel.Users
{
    public class SpamDetectingStrategy
    {
        public ProtectionPunishment SelectPunishment(int warnsInLastFewMinutes, int warnsInLastFewHours, int messagesInLastFewSeconds, int messagesInLastFewMinutes)
        {
            if (messagesInLastFewSeconds >= 5 && messagesInLastFewMinutes >= 50)
            {
                if (warnsInLastFewHours == 0)
                {
                    return new ProtectionPunishment(ProtectionPunishmentOption.Warn);
                }

                if (warnsInLastFewMinutes > 1 && warnsInLastFewHours > 3)
                {
                    return new ProtectionPunishment(ProtectionPunishmentOption.Mute);
                }

                return new ProtectionPunishment(ProtectionPunishmentOption.LongMute);
            }

            return new ProtectionPunishment(ProtectionPunishmentOption.Nothing);
        }
    }
}