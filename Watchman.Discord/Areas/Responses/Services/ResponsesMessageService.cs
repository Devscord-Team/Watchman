using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Watchman.Discord.Areas.Responses.Services
{
    public class ResponsesMessageService
    {
        private const string DESCRIPTION = "dokumentacja:\nhttps://watchman.readthedocs.io/pl/latest/135-services-in-framework/";
        private readonly ResponsesGetterService _responsesDatabase;
        private readonly MessagesServiceFactory _messagesServiceFactory;

        public ResponsesMessageService(ResponsesGetterService responsesDatabase, MessagesServiceFactory messagesServiceFactory)
        {
            this._responsesDatabase = responsesDatabase;
            this._messagesServiceFactory = messagesServiceFactory;
        }

        public async Task PrintResponses(string commandArgument, Contexts contexts)
        {
            var messagesService = this._messagesServiceFactory.Create(contexts);
            if (commandArgument == "default")
            {
                await messagesService.SendEmbedMessage("Domyślne odpowiedzi:", DESCRIPTION, this.GetDefaultResponses());
            }
            else if (commandArgument == "custom")
            {
                await messagesService.SendEmbedMessage("Nadpisane odpowiedzi:", DESCRIPTION, this.GetCustomResponses(contexts.Server.Id));
            }
            else
            {
                await messagesService.SendEmbedMessage("Wszystkie odpowiedzi:", DESCRIPTION, this.GetAllResponses(contexts.Server.Id));
            }
        }

        private IEnumerable<KeyValuePair<string, string>> GetDefaultResponses()
        {
            return this._responsesDatabase.GetResponsesFromBase()
                .Where(x => x.IsDefault)
                .Select(x => new KeyValuePair<string, string>(x.OnEvent, this.GetRawMessage(x.Message)));
        }

        private IEnumerable<KeyValuePair<string, string>> GetCustomResponses(ulong serverId)
        {
            var responses = this._responsesDatabase.GetResponsesFromBase()
                .Where(x => x.ServerId == serverId)
                .Select(x => new KeyValuePair<string, string>(x.OnEvent, this.GetRawMessage(x.Message)));

            if (!responses.Any())
            {
                responses = responses.Append(new KeyValuePair<string, string>("-------------------------------", "```Obecnie jest brak nadpisanych responses!```"));
            }
            return responses;
        }

        private IEnumerable<KeyValuePair<string, string>> GetAllResponses(ulong serverId)
        {
            var responses = this._responsesDatabase.GetResponsesFromBase().ToList();
            var serverResponses = responses.Where(x => x.ServerId == serverId).ToList();
            var notOverwrittenDefaultResponses = responses.Where(response => response.IsDefault && serverResponses.All(s => s.OnEvent != response.OnEvent));
            serverResponses.AddRange(notOverwrittenDefaultResponses);
            return serverResponses.Select(x => new KeyValuePair<string, string>(x.OnEvent, this.GetRawMessage(x.Message)));
        }

        private string GetRawMessage(string message)
        {
            return message.Replace("`", @"\`").Replace("*", @"\*");
        }
    }
}
