﻿using Devscord.DiscordFramework.Services.Factories;
using System.Threading.Tasks;
using Watchman.Cqrs;

namespace Watchman.Discord.Integration.Channels.Commands.Handlers
{
    public class SendMessageToChannelCommandHandler : ICommandHandler<SendMessageToChannelCommand>
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;

        public SendMessageToChannelCommandHandler(MessagesServiceFactory messagesServiceFactory)
        {
            this._messagesServiceFactory = messagesServiceFactory;
        }

        public async Task HandleAsync(SendMessageToChannelCommand command)
        {
            var messagesService = this._messagesServiceFactory.Create(command.ChannelId, command.GuildId);
            await messagesService.SendMessage(command.Message);
        }
    }
}
