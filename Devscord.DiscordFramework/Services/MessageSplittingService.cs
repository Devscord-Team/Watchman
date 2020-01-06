using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCRE;

namespace Devscord.DiscordFramework.Services
{
    public class MessageSplittingService : IService
    {
        const int MAX_MESSAGE_LENGTH = 1990; // for safety reason I made it smaller than 2000

        public IEnumerable<string> SplitMessage(string fullMessage, MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Json:
                    fullMessage = TrimUselessWhitespaceFromBeginning(fullMessage);
                    return SplitJsonMessage(fullMessage);

                case MessageType.NormalText:
                default:
                    return SplitNormalMessage(fullMessage);
            }
        }

        private IEnumerable<string> SplitNormalMessage(string fullMessage)
        {
            var copiedMessage = (string)fullMessage.Clone();
            while (copiedMessage.Length > MAX_MESSAGE_LENGTH)
            {
                var messageInChars = copiedMessage.Take(MAX_MESSAGE_LENGTH).ToList();
                
                var lastIndex = messageInChars.LastIndexOf('\n');
                if (lastIndex < 1)
                {
                    lastIndex = messageInChars.LastIndexOf(' ');
                }

                yield return copiedMessage.Substring(0, lastIndex);
                copiedMessage = copiedMessage.Substring(lastIndex);
            }

            yield return copiedMessage;
        }

        private string TrimUselessWhitespaceFromBeginning(string fullMessage)
        {
            var lines = fullMessage.Split('\n');
            var howManyWhitespaces = lines.First().IndexOf('{');

            return lines.Aggregate((sum, next) => sum + next.Remove(0, howManyWhitespaces));
        }

        private IEnumerable<string> SplitJsonMessage(string fullMessage)
        {
            var matched = new PcreRegex(@"{(?>[^{}]|(?R))*}").Matches(fullMessage).Select(x => x.Value);

            var oneMessage = new StringBuilder();
            foreach (var jsonElement in matched)
            {
                if (jsonElement.Length + oneMessage.Length > MAX_MESSAGE_LENGTH)
                {
                    yield return oneMessage.ToString();
                    oneMessage.Clear();
                }
                oneMessage.AppendLine(jsonElement + ',');
            }

            yield return oneMessage.ToString()[..^3]; // skip last ",\n"
        }
    }
}
