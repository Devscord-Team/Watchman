using Discord.WebSocket;
using System;
using System.Collections.Generic;
using Watchman.Discord.Areas.Statistics.Models;

namespace Watchman.Discord.Areas.Statistics.Builders
{
    public class MessageBuilder
    {
        private MessageInformationAuthor author;
        private MessageInformationChannel channel;
        private MessageInformationServer server;
        private readonly string _message;
        private readonly UserContext _userContext;
        private readonly ChannelContext _channelContext;

        public MessageBuilder(string message, Dictionary<string, IDiscordContext> contexts)
        {
            this._message = message;
            this._userContext = (UserContext) contexts[nameof(UserContext)];
            this._channelContext = (ChannelContext) contexts[nameof(ChannelContext)];
        }

        public MessageBuilder SetAuthor()
        {
            this.author = new MessageInformationAuthor
            {
                Id = _userContext.Id,
                Name = _userContext.Name
            };
            return this;
        }

        public MessageBuilder SetChannel()
        {
            this.channel = new MessageInformationChannel
            {
                Id = _channelContext.Id,
                Name = _channelContext.Name
            };
            return this;
        }

        public MessageBuilder SetServerInfo()
        {
            var serverInfo = ((SocketGuildChannel)Server.GetChannel(_channelContext.Id)).Guild;

            this.server = new MessageInformationServer
            {
                Id = serverInfo.Id,
                Name = serverInfo.Name,
                Owner = new MessageInformationAuthor
                {
                    Id = serverInfo.Owner.Id,
                    Name = serverInfo.Owner.ToString()
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
