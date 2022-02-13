using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.Discord.Areas.Protection.Services.Commands
{
    public class SendMutedUsersDirectMessageCommand : ICommand
    {
        public Contexts Contexts { get; }

        public SendMutedUsersDirectMessageCommand(Contexts contexts)
        {
            this.Contexts = contexts;
        }
    }
}
