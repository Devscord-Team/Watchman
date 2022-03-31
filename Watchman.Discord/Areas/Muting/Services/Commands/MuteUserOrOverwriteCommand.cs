using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Linq;
using System.Text;
using Watchman.Cqrs;
using Watchman.DomainModel.Muting;

namespace Watchman.Discord.Areas.Muting.Services.Commands
{
    public class MuteUserOrOverwriteCommand : ICommand
    {
        public Contexts Contexts { get; }
        public MuteEvent MuteEvent { get; }
        public UserContext UserToMute { get; }

        public MuteUserOrOverwriteCommand(Contexts contexts, MuteEvent muteEvent, UserContext userToMute)
        {
            this.Contexts = contexts;
            this.MuteEvent = muteEvent;
            this.UserToMute = userToMute;
        }
    }
}
