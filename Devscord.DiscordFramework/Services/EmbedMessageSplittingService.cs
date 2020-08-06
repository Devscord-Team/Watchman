using Discord;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Services
{
    public class EmbedMessageSplittingService
    {
        private const int MAX_FIELDS = 25;
        private readonly EmbedMessagesService _embedMessagesService;

        public EmbedMessageSplittingService(EmbedMessagesService embedMessagesService)
        {
            this._embedMessagesService = embedMessagesService;
        }

        internal IEnumerable<Embed> SplitEmbedMessage(string title, string description, IEnumerable<KeyValuePair<string, string>> values)
        {
            var messages = this.SplitMessage(values.ToList()).ToList();
            yield return this._embedMessagesService.Generate(title, description, messages.FirstOrDefault() ?? new Dictionary<string, string>());
            foreach (var message in messages.Skip(1))
            {
                yield return this._embedMessagesService.Generate(title: null, description: null, message);
            }
        }

        private IEnumerable<IEnumerable<KeyValuePair<string, string>>> SplitMessage(IReadOnlyCollection<KeyValuePair<string, string>> values)
        {
            for (var i = 0; i < Math.Ceiling((double) values.Count / MAX_FIELDS); i++)
            {
                yield return values.Skip(MAX_FIELDS * i).Take(MAX_FIELDS);
            }
        }
    }
}
