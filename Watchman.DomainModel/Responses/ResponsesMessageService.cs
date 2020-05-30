using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Resources = Devscord.DiscordFramework.Framework.Commands.Responses.Resources;

namespace Watchman.DomainModel.Responses
{
    public class ResponsesMessageService
    {
        private ResponsesDatabase _responsesDatabase;
        private MessagesService _messagesService;
        private const int MAX_NUMBER_OF_MESSAGE_FIELDS = 20;

        public ResponsesMessageService(ResponsesDatabase responsesDatabase, MessagesService messagesService)
        {
            _responsesDatabase = responsesDatabase;
            _messagesService = messagesService;
        }

        public Task PrintResponses(string commandArgument)
        {
            if (commandArgument == "default")
            {
                PrintDefaultResponses();
            }

            if (commandArgument == "custom")
            {
                PrintCustomResponses();
            }

            if (commandArgument == "all")
            {
                PrintAllResponses();
            }

            return Task.CompletedTask;
        }

        private void PrintDefaultResponses()
        {
            var messagesAboutDefaultResponses = GetMessagesAboutDefaultResponses();
            for (int i = 0; i < messagesAboutDefaultResponses.Count; ++i)
            {
                if (i == 0)
                {
                    _messagesService.SendEmbedMessage("Domyślne responses:", "dokumentacja:\nhttps://watchman.readthedocs.io/pl/latest/135-services-in-framework/", messagesAboutDefaultResponses[0]);
                }

                else
                {
                    _messagesService.SendEmbedMessage(title: null, description: null, messagesAboutDefaultResponses[i]);
                }
            }
        }

        private void PrintCustomResponses()
        {
            var messagesAboutCustomResponses = GetMessagesAboutCustomResponses();
            for (int i = 0; i < messagesAboutCustomResponses.Count; ++i)
            {
                if (i == 0)
                {
                    _messagesService.SendEmbedMessage("Nadpisane responses:", description: null, messagesAboutCustomResponses[0]);
                }

                else
                {
                    _messagesService.SendEmbedMessage(title: null, description: null, messagesAboutCustomResponses[i]);
                }
            }

            if (messagesAboutCustomResponses.Any() == false)
            {
                var message = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("-------------------------------", "```Obecnie jest brak nadpisanych responses!```")
                };
                _messagesService.SendEmbedMessage("Nadpisane responses:", description: null, message);
            }
        }

        private void PrintAllResponses()
        {
            var messagesAboutAllResponses = GetMessagesAboutAllResponses();
            for (int i = 0; i < messagesAboutAllResponses.Count; ++i)
            {
                if (i == 0)
                {
                    _messagesService.SendEmbedMessage("Wszystkie responses:", description: null, messagesAboutAllResponses[0]);
                }

                else
                {
                    _messagesService.SendEmbedMessage(title: null, description: null, messagesAboutAllResponses[i]);
                }
            }
        }

        private List<List<KeyValuePair<string, string>>> GetMessagesAboutDefaultResponses()
        {
            var defaultResponses = _responsesDatabase.GetResponsesFromBase()
                                   .Where(x => x.IsDefault)
                                   .ToList();
            var messagesAboutDefaultResponses = new List<List<KeyValuePair<string, string>>>();

            for (int i = 0; i < defaultResponses.Count; ++i)
            {
                if (i % MAX_NUMBER_OF_MESSAGE_FIELDS == 0)
                {
                    messagesAboutDefaultResponses.Add(new List<KeyValuePair<string, string>>(MAX_NUMBER_OF_MESSAGE_FIELDS));
                }

                var onEvent = defaultResponses[i].OnEvent;
                var message = GetMessageWithoutFramesAndBold(defaultResponses[i].Message);
                var oneField = new KeyValuePair<string, string>(onEvent, message);
                messagesAboutDefaultResponses.Last().Add(oneField);
            }

            return messagesAboutDefaultResponses;
        }

        private List<List<KeyValuePair<string, string>>> GetMessagesAboutCustomResponses()
        {
            var customResponses = _responsesDatabase.GetResponsesFromBase()
                                  .Where(x => !x.IsDefault)
                                  .ToList();
            var messagesAboutCustomResponses = new List<List<KeyValuePair<string, string>>>();

            for (int i = 0; i < customResponses.Count; ++i)
            {
                if (i % MAX_NUMBER_OF_MESSAGE_FIELDS == 0)
                {
                    messagesAboutCustomResponses.Add(new List<KeyValuePair<string, string>>(MAX_NUMBER_OF_MESSAGE_FIELDS));
                }

                var onEvent = customResponses[i].OnEvent;
                var message = GetMessageWithoutFramesAndBold(customResponses[i].Message);
                var oneField = new KeyValuePair<string, string>(onEvent, message);
                messagesAboutCustomResponses.Last().Add(oneField);
            }

            return messagesAboutCustomResponses;
        }

        private List<List<KeyValuePair<string, string>>> GetMessagesAboutAllResponses()
        {
            var responses = _responsesDatabase.GetResponsesFromBase().ToList();
            var messagesAboutAllResponses = new List<List<KeyValuePair<string, string>>>();

            for (int i = 0; i < responses.Count; ++i)
            {
                if (i % MAX_NUMBER_OF_MESSAGE_FIELDS == 0)
                {
                    messagesAboutAllResponses.Add(new List<KeyValuePair<string, string>>(MAX_NUMBER_OF_MESSAGE_FIELDS));
                }

                var onEvent = responses[i].OnEvent;
                var message = GetMessageWithoutFramesAndBold(responses[i].Message);
                var oneField = new KeyValuePair<string, string>(onEvent, message);
                messagesAboutAllResponses.Last().Add(oneField);
            }

            return messagesAboutAllResponses;
        }

        private string GetMessageWithoutFramesAndBold(string message)
        {
            message = message.Contains('`')
                      ? message.Insert(message.IndexOf("`"), "\\")
                      : message;

            message = message.Contains("*")
                      ? message.Insert(message.IndexOf("*"), "\\")
                      : message;

            return message;
        }
    }
}
