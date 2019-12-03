using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.DomainModel.Protection.Services
{
    public class AntiSpamDomainStrategy
    {
        public ProtectionPunishment SelectPunishment(bool isAlerted, int messagesInLast10Seconds, int mutesInPast)
        {
            if(!isAlerted && messagesInLast10Seconds >= 5)
            {
                return new ProtectionPunishment(ProtectionPunishmentOptions.Alert);
            }
            if(isAlerted && messagesInLast10Seconds >= 10)
            {
                if(mutesInPast > 4)
                {
                    return new ProtectionPunishment(ProtectionPunishmentOptions.Ban);
                }
                return new ProtectionPunishment(ProtectionPunishmentOptions.Mute, TimeSpan.FromMinutes(15 * (mutesInPast + 1)));
            }
            if(isAlerted && messagesInLast10Seconds == 0)
            {
                return new ProtectionPunishment(ProtectionPunishmentOptions.Clear);
            }

            return new ProtectionPunishment(ProtectionPunishmentOptions.Nothing);
        }
    }
}
