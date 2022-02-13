using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.Discord.Areas.Protection.Services.Commands
{
    public class MuteAgainIfNeededCommand : ICommand
    {
        public Contexts Contexts { get; }

        public MuteAgainIfNeededCommand(Contexts contexts)
        {
            this.Contexts = contexts;
        }
    }
}
