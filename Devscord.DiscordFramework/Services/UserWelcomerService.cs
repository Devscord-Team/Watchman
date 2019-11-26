using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Services
{
    public class UserWelcomerService
    {
        public Task WelcomeUser(ChannelContext channelContext, UserContext userContext, DiscordServerContext serverContext)
        {
            var messageService = new MessagesService()
            {
                DefaultChannelId = channelContext.Id
            };
            messageService.SendMessage($"Witaj {userContext.Name} na serwerze {serverContext.Name}");
            return Task.CompletedTask;
        }
    }
}
