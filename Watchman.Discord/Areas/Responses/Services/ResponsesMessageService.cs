using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Devscord.DiscordFramework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.DomainModel.Responses;
using Watchman.Discord.Areas.Responses.BotCommands;

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

        public Task PrintResponses(ResponsesCommand command, Contexts contexts)
        {
            var messagesService = this._messagesServiceFactory.Create(contexts);
            if (command.Default)
            {
                return messagesService.SendEmbedMessage("Domyślne odpowiedzi:", DESCRIPTION, this.GetDefaultResponses());
            }
            else if (command.Custom)
            {
                return messagesService.SendEmbedMessage("Nadpisane odpowiedzi:", DESCRIPTION, this.GetCustomResponses(contexts.Server.Id));
            }
            return messagesService.SendEmbedMessage("Wszystkie odpowiedzi:", DESCRIPTION, this.GetAllResponses(contexts.Server.Id)); 
        }

        private IEnumerable<KeyValuePair<string, string>> GetDefaultResponses()
        {
            return this._responsesDatabase.GetResponsesFromBase()
                .Where(x => x.IsDefault)
                .Select(x => new KeyValuePair<string, string>(x.OnEvent, this.GetResponseWithVariableList(x)));
        }

        private IEnumerable<KeyValuePair<string, string>> GetCustomResponses(ulong serverId)
        {
            var responses = this._responsesDatabase.GetResponsesFromBase()
                .Where(x => x.ServerId == serverId)
                .Select(x => new KeyValuePair<string, string>(x.OnEvent, this.GetResponseWithVariableList(x)));

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
            return serverResponses.Select(x => new KeyValuePair<string, string>(x.OnEvent, this.GetResponseWithVariableList(x)));
        }

        private string GetResponseWithVariableList(Response response)
        {
            var result = "\n__Dostępne zmienne:__";
            if (response.AvailableVariables.Any())
            {
                result += response.AvailableVariables.Select(s => $" `{s}`").Aggregate((a, b) => a + b);
            }
            else
            {
                result += " brak";
            }

            return this.GetRawMessage(response.Message) + result;
        }
        
        private string GetRawMessage(string message)
        {
            return message.Replace("`", @"\`").Replace("*", @"\*");
        }
    }
}
