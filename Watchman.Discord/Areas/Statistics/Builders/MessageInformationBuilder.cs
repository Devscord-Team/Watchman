using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using Watchman.Discord.Areas.Statistics.Models;

namespace Watchman.Discord.Areas.Statistics.Builders
{
    public class MessageInformationBuilder
    {
        private MessageInformationAuthor author;
        private MessageInformationChannel channel;
        private MessageInformationServer server;
        private string _message;

        public MessageInformationBuilder(string message)
        {
            this._message = message;
        }

        public MessageInformationBuilder SetAuthor(UserContext user)
        {
            this.author = new MessageInformationAuthor
            {
                Id = user.Id,
                Name = user.Name
            };
            return this;
        }

        public MessageInformationBuilder SetChannel(ChannelContext channel)
        {
            this.channel = new MessageInformationChannel
            {
                Id = channel.Id,
                Name = channel.Name
            };
            return this;
        }

        public MessageInformationBuilder SetServerInfo(DiscordServerContext server)
        {
            this.server = new MessageInformationServer
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
                Author = author,
                Channel = channel,
                Server = server,
                Content = _message,
                Date = date
            };
        }
    }
}
