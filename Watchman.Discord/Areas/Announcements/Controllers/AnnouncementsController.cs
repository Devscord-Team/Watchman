using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Watchman.Discord.Framework.Architecture.Controllers;
using Watchman.Discord.Framework.Architecture.Middlewares;
using Watchman.Integrations.Disboard;

namespace Watchman.Discord.Areas.Announcements.Controllers
{
    class AnnouncementsController : IController
    {
        private readonly ServerBumper _bumper;

        public AnnouncementsController()
        {
            _bumper = new ServerBumper();
        }

        [AdminCommand]
        [DiscordCommand("-autobump start")]
        public void AutoBumpStart(string message, Dictionary<string, IDiscordContext> contexts)
        {
            var isStarted = _bumper.AddServerChannel(socketMessage.Channel);

            if (isStarted.Result)
            {
                socketMessage.Channel.SendMessageAsync("Rozpoczęto automatyczne podbijanie.");
            }
        }

        [AdminCommand]
        [DiscordCommand("-autobump stop")]
        public void AutoBumpStop(string message, Dictionary<string, IDiscordContext> contexts)
        {
            var isStopped = _bumper.RemoveServerChannel(socketMessage.Channel);

            if (isStopped.Result)
            {
                socketMessage.Channel.SendMessageAsync("Zatrzymano automatyczne podbijanie.");
            }
        }
    }
}
