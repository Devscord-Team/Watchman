using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devscord.DiscordFramework.Services
{
    public class MessagesServiceFactory : IService
    {
        public MessagesService Create(Contexts contexts)
        {
            return new MessagesService
            {
                ChannelId = contexts.Channel.Id
            };
        }

        public MessagesService Create(Contexts contexts, ResponsesService responsesService)
        {
            return new MessagesService(responsesService)
            {
                ChannelId = contexts.Channel.Id
            };
        }
    }
}
