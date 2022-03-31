using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Text;
using Watchman.Cqrs;
using Watchman.DomainModel.Muting;

namespace Watchman.Discord.Areas.Muting.Services.Commands
{
    public class UnmuteSpecificEventCommand : ICommand
    {
        public Contexts Contexts { get; }
        public MuteEvent MuteEvent { get; }
        public UserContext UserToUnmute { get; }

        public UnmuteSpecificEventCommand(Contexts contexts, MuteEvent muteEvent, UserContext UserToUnmute)
        {
            this.Contexts = contexts;
            this.MuteEvent = muteEvent;
            this.UserToUnmute = UserToUnmute;
        }
    }
}
