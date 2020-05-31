using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Watchman.Discord.Areas.Responses.Services
{
    public class ResponsesMessageService
    {
        private const string DESCRIPTION = "dokumentacja:\nhttps://watchman.readthedocs.io/pl/latest/135-services-in-framework/";
        private readonly ResponsesGetterService _responsesDatabase;
        private readonly EmbedMessageSplittingService _embedMessageSplittingService;

        public ResponsesMessageService(ResponsesGetterService responsesDatabase, EmbedMessageSplittingService embedMessageSplittingService)
        {
            _responsesDatabase = responsesDatabase;
            _embedMessageSplittingService = embedMessageSplittingService;
        }

        public async Task PrintResponses(string commandArgument, ulong serverId)
        {
            if (commandArgument == "default")
            {
                await _embedMessageSplittingService.SendEmbedSplitMessage("Domyślne responses:", DESCRIPTION, GetDefaultResponses());
            }
            else if (commandArgument == "custom")
            {
                await _embedMessageSplittingService.SendEmbedSplitMessage("Nadpisane responses:", DESCRIPTION, GetCustomResponses(serverId));
            }
            else
            {
                await _embedMessageSplittingService.SendEmbedSplitMessage("Wszystkie responses:", DESCRIPTION, GetAllResponses());
            }
        }

        private IEnumerable<KeyValuePair<string, string>> GetDefaultResponses()
        {
            var responses = _responsesDatabase.GetResponsesFromBase()
                            .Where(x => x.IsDefault)
                            .ToList();

            foreach (var response in responses)
            {
                yield return new KeyValuePair<string, string>(response.OnEvent, GetMessageWithoutFramesAndBold(response.Message));
            }
        }

        private IEnumerable<KeyValuePair<string, string>> GetCustomResponses(ulong serverId)
        {
            var responses = _responsesDatabase.GetResponsesFromBase()
                            .Where(x => !x.IsDefault)
                            .Where(x => x.ServerId == serverId)
                            .ToList();

            if (responses.Any() == false)
            {
                yield return new KeyValuePair<string, string>("-------------------------------", "```Obecnie jest brak nadpisanych responses!```");
            }
            foreach (var response in responses)
            {
                yield return new KeyValuePair<string, string>(response.OnEvent, GetMessageWithoutFramesAndBold(response.Message));
            }
        }

        private IEnumerable<KeyValuePair<string, string>> GetAllResponses()
        {
            var responses = _responsesDatabase.GetResponsesFromBase().ToList();

            foreach (var response in responses)
            {
                yield return new KeyValuePair<string, string>(response.OnEvent, GetMessageWithoutFramesAndBold(response.Message));
            }
        }

        private string GetMessageWithoutFramesAndBold(string message)
        {
            message = message.Contains("`")
                      ? message.Insert(message.IndexOf("`"), "\\")
                      : message;

            message = message.Contains("*")
                      ? message.Insert(message.IndexOf("*"), "\\")
                      : message;

            return message;
        }
    }
}
