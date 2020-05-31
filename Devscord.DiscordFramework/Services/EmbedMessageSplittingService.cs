using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Services
{
    public class EmbedMessageSplittingService
    {
        private const int MAX_NUMBER_OF_MESSAGE_FIELDS = 20;
        private MessagesService _messagesService;

        public EmbedMessageSplittingService(MessagesService messagesService)
        {
            _messagesService = messagesService;
        }

        public async Task SendEmbedSplitMessage(string title, string description, IEnumerable<KeyValuePair<string, string>> values)
        {
            var messages = SplitMessage(values.ToList());

            await _messagesService.SendEmbedMessage(title, description, messages[0]);
            for (int i = 1; i < messages.Count; ++i)
            {
                await _messagesService.SendEmbedMessage(title: null, description: null, messages[i]);
            }
        }

        private List<List<KeyValuePair<string, string>>> SplitMessage(List<KeyValuePair<string, string>> values)
        {
            var messages = new List<List<KeyValuePair<string, string>>>();
            for (int i = 0; i < values.Count; ++i)
            {
                if (i % MAX_NUMBER_OF_MESSAGE_FIELDS == 0)
                {
                    messages.Add(new List<KeyValuePair<string, string>>(MAX_NUMBER_OF_MESSAGE_FIELDS));
                }

                messages.Last().Add(values[i]);
            }

            return messages;
        }
    }
}
