﻿using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Integration;
using Discord.Rest;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons.Exceptions;
using System.Linq;

namespace Devscord.DiscordFramework.Services
{
    public class MessagesService : ICyclicService
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }

        private static readonly Dictionary<ulong, IEnumerable<Response>> _serversResponses = new Dictionary<ulong, IEnumerable<Response>>();
        private readonly ResponsesService _responsesService;
        private readonly MessageSplittingService _splittingService;
        private readonly EmbedMessagesService _embedMessagesService;

        public MessagesService(ResponsesService responsesService, MessageSplittingService splittingService, EmbedMessagesService embedMessagesService)
        {
            this._responsesService = responsesService;
            this._splittingService = splittingService;
            this._embedMessagesService = embedMessagesService;
            if (_serversResponses.Count == 0)
            {
                this.Refresh().Wait();
            }
        }

        public async Task SendMessage(string message, MessageType messageType = MessageType.NormalText)
        {
            var channel = this.GetChannel();
            foreach (var mess in this._splittingService.SplitMessage(message, messageType))
            {
                await channel.SendMessageAsync(mess);
                Log.Information("Bot sent message {splitted} {message}", mess, messageType != MessageType.NormalText ? "splitted" : string.Empty);
            }
        }

        public Task SendEmbedMessage(string title, string description, IEnumerable<KeyValuePair<string, string>> values)
        {
            var channel = this.GetChannel();
            var embed = this._embedMessagesService.Generate(title, description, values);
            channel.SendMessageAsync(embed: embed);
            return Task.CompletedTask;
        }

        public Task SendResponse(Func<ResponsesService, string> response)
        {
            this._responsesService.Responses = _serversResponses.GetValueOrDefault(this.GuildId) ?? this.GetResponsesForNewServer(this.GuildId);
            var message = response.Invoke(this._responsesService);
            return this.SendMessage(message);
        }

        public async Task SendFile(string filePath)
        {
            var channel = this.GetChannel();
            await channel.SendFileAsync(filePath);
        }

        public async Task SendFile(string fileName, Stream stream)
        {
            var channel = this.GetChannel();
            await channel.SendFileAsync(stream, fileName);
            await stream.DisposeAsync();
        }

        public async Task SendExceptionResponse(BotException botException)
        {
            var responseName = botException.GetType().Name.Replace("Exception", "");
            var responseManagerMethod = typeof(ResponsesManager).GetMethod(responseName);
            if (responseManagerMethod == null)
            {
                Log.Error("{name} doesn't exists as a response", responseName);
                await this.SendMessage($"{responseName} doesn't exists as a response"); // message typed into code, bcs it's called only when there is a problem with responses
                return;
            }
            await this.SendResponse(x =>
            {
                var arg = new object[] { x };
                if (botException.Value != null)
                {
                    arg = arg.Append(botException.Value).ToArray();
                }
                return (string) responseManagerMethod.Invoke(null, arg);
            });
        }

        public Task Refresh()
        {
            Log.Information("Refreshing responses cache...");
            foreach (var serverId in _serversResponses.Keys.ToList())
            {
                var responses = this._responsesService.GetResponsesFunc(serverId);
                _serversResponses[serverId] = responses;
            }
            Log.Information("Responses cache refreshed");
            return Task.CompletedTask;
        }

        private IEnumerable<Response> GetResponsesForNewServer(ulong serverId)
        {
            var responses = this._responsesService.GetResponsesFunc(serverId).ToList();
            _serversResponses.Add(serverId, responses);
            return responses;
        }

        private IRestMessageChannel GetChannel()
        {
            RestGuild guild = null;
            if (this.GuildId != default)
            {
                guild = Server.GetGuild(this.GuildId).Result;
            }
            var channel = (IRestMessageChannel) Server.GetChannel(this.ChannelId, guild).Result;
            return channel;
        }
    }
}
