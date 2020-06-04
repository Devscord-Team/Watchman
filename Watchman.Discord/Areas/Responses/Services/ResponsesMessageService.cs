using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Watchman.Discord.Areas.Responses.Services
{
    public class ResponsesMessageService
    {
        private const string DESCRIPTION = "dokumentacja:\nhttps://watchman.readthedocs.io/pl/latest/135-services-in-framework/";
        private readonly ResponsesGetterService _responsesDatabase;
        private readonly EmbedMessageSplittingService _embedMessageSplittingService;

        public ResponsesMessageService(ResponsesGetterService responsesDatabase, EmbedMessageSplittingService embedMessageSplittingService)
        {
            this._responsesDatabase = responsesDatabase;
            this._embedMessageSplittingService = embedMessageSplittingService;
        }

        public async Task PrintResponses(string commandArgument, Contexts contexts)
        {
            Func<Task> sendMessages = commandArgument switch
            {
                "default" => async () => await _embedMessageSplittingService.SendEmbedSplitMessage("Domyślne responses:", DESCRIPTION, GetDefaultResponses(), contexts),
                "custom" => async () => await _embedMessageSplittingService.SendEmbedSplitMessage("Nadpisane responses:", DESCRIPTION, GetCustomResponses(contexts.Server.Id), contexts),
                _ => async () => await _embedMessageSplittingService.SendEmbedSplitMessage("Wszystkie responses:", DESCRIPTION, GetAllResponses(), contexts),
            };
            await sendMessages();
        }

        private IEnumerable<KeyValuePair<string, string>> GetDefaultResponses()
        {
            return _responsesDatabase.GetResponsesFromBase()
                .Where(x => x.IsDefault)
                .Select(x => new KeyValuePair<string, string>(x.OnEvent, GetRawMessage(x.Message)));
        }

        private IEnumerable<KeyValuePair<string, string>> GetCustomResponses(ulong serverId)
        {
            var responses = _responsesDatabase.GetResponsesFromBase()
                .Where(x => x.ServerId == serverId)
                .Select(x => new KeyValuePair<string, string>(x.OnEvent, GetRawMessage(x.Message)));

            if (!responses.Any())
            {
                responses = responses.Append(new KeyValuePair<string, string>("-------------------------------", "```Obecnie jest brak nadpisanych responses!```"));
            }
            return responses;
        }

        private IEnumerable<KeyValuePair<string, string>> GetAllResponses()
        {
            return _responsesDatabase.GetResponsesFromBase()
                .Select(x => new KeyValuePair<string, string>(x.OnEvent, GetRawMessage(x.Message)));
        }

        private string GetRawMessage(string message)
        {
            return message.Replace("`", @"\`").Replace("*", @"\*");
        }
    }
}
