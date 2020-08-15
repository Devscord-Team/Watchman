using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Discord;

namespace Devscord.DiscordFramework.Services
{
    public class EmbedMessageSplittingService
    {
        private const int MAX_FIELDS = 25;
        private const int MAX_FIELD_LENGTH = 950;
        private const int MAX_EMBED_LENGTH = 5500; // set less than 6000 for safety
        private readonly EmbedMessagesService _embedMessagesService;

        public EmbedMessageSplittingService(EmbedMessagesService embedMessagesService)
        {
            this._embedMessagesService = embedMessagesService;
        }

        internal IEnumerable<Embed> SplitEmbedMessage(string title, string description, IEnumerable<KeyValuePair<string, string>> values)
        {
            var leftForValues = MAX_EMBED_LENGTH - title.Length - description.Length;
            var messages = this.SplitMessage(values, leftForValues).ToList();
            yield return this._embedMessagesService.Generate(title, description, messages.FirstOrDefault() ?? new Dictionary<string, string>());
            foreach (var message in messages.Skip(1))
            {
                yield return this._embedMessagesService.Generate(title: null, description: null, message);
            }
        }

        internal IEnumerable<Embed> SplitEmbedMessage(string title, string description, IEnumerable<KeyValuePair<string, Dictionary<string, string>>> values)
        {
            var leftForValues = MAX_EMBED_LENGTH - title.Length - description.Length - this._embedMessagesService.FooterLength;
            var messages = this.SplitMessage(values, leftForValues).ToList();
            yield return this._embedMessagesService.Generate(title, description, messages.FirstOrDefault() ?? new Dictionary<string, Dictionary<string, string>>());
            foreach (var message in messages.Skip(1))
            {
                yield return this._embedMessagesService.Generate(title: null, description: null, message);
            }
        }

        private IEnumerable<IEnumerable<KeyValuePair<string, string>>> SplitMessage(IEnumerable<KeyValuePair<string, string>> values, int lengthLeftForValues)
        {
            var valuesToReturn = new List<KeyValuePair<string, string>>();
            var valuesToReturnLength = 0;
            foreach (var value in values)
            {
                var valueLength = value.Key.Length + value.Value.Length;
                var valuesLengthPlusNewValue = valuesToReturnLength + valueLength;
                if (valuesLengthPlusNewValue > lengthLeftForValues || valuesToReturn.Count == MAX_FIELDS)
                {
                    yield return valuesToReturn;
                    valuesToReturn = new List<KeyValuePair<string, string>>();
                    valuesToReturnLength = 0;
                }
                valuesToReturn.Add(value);
                valuesToReturnLength += valueLength;
            }
            yield return valuesToReturn;
        }

        private IEnumerable<IEnumerable<KeyValuePair<string, Dictionary<string, string>>>> SplitMessage(IEnumerable<KeyValuePair<string, Dictionary<string, string>>> values, int lengthLeftForValues)
        {
            var valuesToReturn = new List<KeyValuePair<string, Dictionary<string, string>>>();
            var valuesToReturnLength = 0;
            foreach (var (subtitle, lines) in values)
            {
                var linesToReturn = new Dictionary<string, string>();
                foreach (var (leftPart, rightPart) in lines)
                {
                    var lineLength = leftPart.Length + rightPart.Length;
                    var valuesPlusLineLength = valuesToReturnLength + lineLength;
                    if (valuesPlusLineLength > lengthLeftForValues || valuesPlusLineLength > MAX_FIELD_LENGTH)
                    {
                        valuesToReturn.Add(new KeyValuePair<string, Dictionary<string, string>>(subtitle, linesToReturn));
                        yield return valuesToReturn;
                        valuesToReturn = new List<KeyValuePair<string, Dictionary<string, string>>>();
                        linesToReturn = new Dictionary<string, string>();
                        valuesToReturnLength = 0;
                    }
                    linesToReturn.Add(leftPart, rightPart);
                    valuesToReturnLength += lineLength;
                }
                valuesToReturn.Add(new KeyValuePair<string, Dictionary<string, string>>(subtitle, linesToReturn));

                if (valuesToReturn.Count == MAX_FIELDS)
                {
                    yield return valuesToReturn;
                    valuesToReturn = new List<KeyValuePair<string, Dictionary<string, string>>>();
                    valuesToReturnLength = 0;
                }
            }
            yield return valuesToReturn;
        }
    }
}
