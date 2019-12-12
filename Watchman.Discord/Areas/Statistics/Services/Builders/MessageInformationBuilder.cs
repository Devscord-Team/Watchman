using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using Watchman.Discord.Areas.Statistics.Models;

namespace Watchman.Discord.Areas.Statistics.Services.Builders
{
    public class MessageInformationBuilder
    {
        private MessageInformationAuthor _author;
        private MessageInformationChannel _channel;
        private MessageInformationServer _server;
        private readonly string _message;

        public MessageInformationBuilder(string message)
        {
            this._message = message;
        }

        public MessageInformationBuilder SetAuthor(UserContext user)
        {
            _author = new MessageInformationAuthor
            {
                Id = user.Id,
                Name = user.Name
            };
            return this;
        }

        public MessageInformationBuilder SetChannel(ChannelContext channel)
        {
            this._channel = new MessageInformationChannel
            {
                Id = channel.Id,
                Name = channel.Name
            };
            return this;
        }

        public MessageInformationBuilder SetServerInfo(DiscordServerContext server)
        {
            this._server = new MessageInformationServer
            {
                Id = server.Id,
                Name = server.Name,
                Owner = new MessageInformationAuthor
                {
                    Id = server.Owner.Id,
                    Name = server.Owner.Name
                }
            };
            return this;
        }

        public MessageInformation Build()
        {
            var date = DateTime.UtcNow;
            return new MessageInformation
            {
                Author = _author,
                Channel = _channel,
                Server = _server,
                Content = _message,
                Date = date
            };
        }
    }
}
