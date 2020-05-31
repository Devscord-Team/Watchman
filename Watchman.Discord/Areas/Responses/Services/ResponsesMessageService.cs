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
        private ResponsesGetterService _responsesDatabase;
        private EmbedMessageSplittingService _embedMessageSplittingService;
        private readonly ulong _serverId;

        public ResponsesMessageService(ResponsesGetterService responsesDatabase, EmbedMessageSplittingService embedMessageSplittingService, ulong serverId)
        {
            _responsesDatabase = responsesDatabase;
            _embedMessageSplittingService = embedMessageSplittingService;
            _serverId = serverId;
        }

        public async Task PrintResponses(string commandArgument)
        {
            if (commandArgument == "default")
            {
                await _embedMessageSplittingService.SendEmbedSplitMessage("Domyślne responses:", DESCRIPTION, GetAppropriateResponses("default"));
            }
            else if (commandArgument == "custom")
            {
                await _embedMessageSplittingService.SendEmbedSplitMessage("Nadpisane responses:", DESCRIPTION, GetAppropriateResponses("custom"));
            }
            else
            {
                await _embedMessageSplittingService.SendEmbedSplitMessage("Wszystkie responses:", DESCRIPTION, GetAppropriateResponses("all"));
            }
        }

        private IEnumerable<KeyValuePair<string, string>> GetAppropriateResponses(string category)
        {
            List<DomainModel.Responses.Response> responses;
            if (category == "default")
            {
                responses = _responsesDatabase.GetResponsesFromBase()
                            .Where(x => x.IsDefault)
                            .ToList();
            }
            if (category == "custom")
            {
                responses = _responsesDatabase.GetResponsesFromBase()
                            .Where(x => !x.IsDefault)
                            .Where(x => x.ServerId == _serverId)
                            .ToList();

                if (responses.Any() == false)
                {
                    yield return new KeyValuePair<string, string>("-------------------------------", "```Obecnie jest brak nadpisanych responses!```");
                }
            }
            else
            {
                responses = _responsesDatabase.GetResponsesFromBase().ToList();
            }

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
