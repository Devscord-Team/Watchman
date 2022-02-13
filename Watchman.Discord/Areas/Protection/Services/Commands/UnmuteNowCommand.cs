using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.Discord.Areas.Protection.Services.Commands
{
    public class UnmuteNowCommand : ICommand
    {
        public Contexts Contexts { get; }
        public UserContext UserToUnmute { get; }

        public UnmuteNowCommand(Contexts contexts, UserContext userToUnmute)
        {
            this.Contexts = contexts;
            this.UserToUnmute = userToUnmute;
        }
    }
}
