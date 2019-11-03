using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Watchman.Discord.Framework.Architecture.Controllers;
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

        [DiscordCommand("-autobump start")]
        public void AutoBumpStart(SocketMessage socketMessage)
        {
            var isStarted = _bumper.AddServerChannel(socketMessage.Channel);

            if (isStarted.Result)
            {
                socketMessage.Channel.SendMessageAsync("Rozpoczęto automatyczne podbijanie.");
            }
        }

        [DiscordCommand("-autobump stop")]
        public void AutoBumpStop(SocketMessage socketMessage)
        {
            var isStopped = _bumper.RemoveServerChannel(socketMessage.Channel);

            if (isStopped.Result)
            {
                socketMessage.Channel.SendMessageAsync("Zatrzymano automatyczne podbijanie.");
            }
        }
    }
}
