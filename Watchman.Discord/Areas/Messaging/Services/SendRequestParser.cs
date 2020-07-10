using System;
using System.Linq;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Watchman.Discord.Areas.Commons;

namespace Watchman.Discord.Areas.Messaging.Services
{
    public class SendRequestParser
    {
        private DiscordRequest _request;
        private Contexts _contexts;
        private ChannelsService _channelsService;

        public SendRequestParser(DiscordRequest request, Contexts contexts, ChannelsService channelsService) {
            this._request = request;
            this._contexts = contexts;
            this._channelsService = channelsService;
        }

        public ChannelContext GetChannel()
        {
            var channel = _request.GetMention();
            var channelToSendMessageTo = this._channelsService.GetChannelByMention(this._contexts.Server, channel);

            if (channelToSendMessageTo==null)
            {
                throw new ChannelNotFoundException(channel);
            }
            return channelToSendMessageTo;
        }

        public string ParseMessage()
        {
            var message = _request.Arguments.Skip(1).FirstOrDefault();
            
            return message?.Value;
        }
    }
}