using Devscord.DiscordFramework.Services.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Services
{
    public class EmbedMessageSplittingService
    {
        private const int MAX_FIELDS = 25;
        private MessagesServiceFactory _messagesServiceFactory;

        public EmbedMessageSplittingService(MessagesServiceFactory messagesServiceFactory)
        {
            this._messagesServiceFactory = messagesServiceFactory;
        }

        public async Task SendEmbedSplitMessage(string title, string description, IEnumerable<KeyValuePair<string, string>> values, Contexts contexts)
        {
            var messagesService = this._messagesServiceFactory.Create(contexts);
            var messages = SplitMessage(values.ToList());

            await messagesService.SendEmbedMessage(title, description, messages.First());
            foreach (var message in messages.Skip(1))
            {
                await messagesService.SendEmbedMessage(title: null, description: null, message);
            }
        }

        private IEnumerable<IEnumerable<KeyValuePair<string, string>>> SplitMessage(List<KeyValuePair<string, string>> values)
        {
            for (int i = 0; i < Math.Ceiling((double)values.Count / MAX_FIELDS); i++)
            {
                yield return values.Skip(MAX_FIELDS * i).Take(MAX_FIELDS);
            }
        }
    }
}
