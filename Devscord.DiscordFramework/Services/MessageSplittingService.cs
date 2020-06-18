using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commons.Extensions;
using PCRE;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devscord.DiscordFramework.Services
{
    public class MessageSplittingService
    {
        private const int MAX_MESSAGE_LENGTH = 1990; // for safety reason I made it smaller than 2000

        public IEnumerable<string> SplitMessage(string fullMessage, MessageType messageType)
        {
            if (fullMessage.Length < MAX_MESSAGE_LENGTH)
            {
                return new List<string> { fullMessage };
            }

            switch (messageType)
            {
                case MessageType.Json:
                    fullMessage = this.TrimUselessWhitespaceFromBeginning(fullMessage);
                    return this.SplitJsonMessage(fullMessage);

                case MessageType.BlockFormatted:
                    return this.SplitBlockMessage(fullMessage);

                case MessageType.NormalText:
                default:
                    return this.SplitNormalMessage(fullMessage);
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
                    yield return oneMessage.FormatMessageIntoBlock("json").ToString();
                    oneMessage.Clear();
                }
                oneMessage.AppendLine(jsonElement + ',');
            }

            var lastMessage = oneMessage.FormatMessageIntoBlock("json").ToString();
            yield return lastMessage.Remove(lastMessage.LastIndexOf(','), 1); // remove the last comma - ','
        }

        private IEnumerable<string> SplitBlockMessage(string fullMessage)
        {
            var blockPrefix = this.GetBlockPrefix(fullMessage);
            var cutMessage = fullMessage.CutStart(blockPrefix).TrimEnd('`', ' ', '\n', '\r'); // ```cs message ```
            var splitMessages = this.SplitNormalMessage(cutMessage)
                .Select(message => blockPrefix + message + "```");
            return splitMessages;
        }

        private string GetBlockPrefix(string fullMessage)
        {
            var firstSpaceIndex = fullMessage.IndexOfAny(" \n\r".ToCharArray());
            var prefix = fullMessage[..firstSpaceIndex];
            return prefix;
        }
    }
}
