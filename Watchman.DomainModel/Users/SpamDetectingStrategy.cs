namespace Watchman.DomainModel.Users
{
    public class SpamDetectingStrategy
    {
        public ProtectionPunishment SelectPunishment(int warnsInLastFewMinutes, int warnsInLastFewHours, int mutesInLastFewHours, int messagesInLastFewSeconds, int messagesInLastFewMinutes, int userMessages)
        {
            const int MIN_MESSAGES_TO_BE_SAFE = 800;

            if (userMessages > MIN_MESSAGES_TO_BE_SAFE)
            {
                return new ProtectionPunishment(ProtectionPunishmentOption.Nothing);
            }

            var badUser = messagesInLastFewSeconds > 10
                          || messagesInLastFewMinutes > 100
                          || (userMessages < 40 && messagesInLastFewSeconds > 5)
                          || (userMessages < 200 && messagesInLastFewMinutes > 40);

            if (!badUser)
            {
                return new ProtectionPunishment(ProtectionPunishmentOption.Nothing);
            }

            if (mutesInLastFewHours >= 1 && warnsInLastFewMinutes > 2)
            {
                return new ProtectionPunishment(ProtectionPunishmentOption.LongMute);
            }

            if (warnsInLastFewMinutes > 1 && warnsInLastFewHours > 3)
            {
                return new ProtectionPunishment(ProtectionPunishmentOption.Mute);
            }

            return new ProtectionPunishment(ProtectionPunishmentOption.Warn);
        }
    }
}