using Watchman.AntiSpam.Models;

namespace Watchman.AntiSpam
{
    public class AntiSpamMessagesPackage
    {
        //todo consider array
        public List<AntiSpamMessage> AntiSpamMessages { get; }

        public AntiSpamMessagesPackage(List<AntiSpamMessage> antiSpamMessages)
        {
            this.AntiSpamMessages = antiSpamMessages;
        }
    }
}
