using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Watchman.Discord.Framework.Architecture.Controllers;
using Watchman.Integrations.Disboard;

namespace Watchman.Discord.Areas.Announcements.Controllers
{
    class AnnouncementsController : ReadAlways, IController
    {
        private TimeSpan HowOften => new TimeSpan(0, 0, 15); // every 2 hours and 1 minute
        private bool _autoBumpIsOn;
        private ServerBumper _bumper;

        [DiscordCommand("-autobump start")]
        public void AutoBumpStart(SocketMessage socketMessage)
        {
            _autoBumpIsOn = true;
            socketMessage.Channel.SendMessageAsync("Started auto bumping.");

            _bumper = new ServerBumper(socketMessage.Channel);
            ManageBumping();
        }

        [DiscordCommand("-autobump stop")]
        public void AutoBumpStop(SocketMessage socketMessage)
        {
            _autoBumpIsOn = false;
            socketMessage.Channel.SendMessageAsync("Stopped auto bumping.");
        }

        private async void ManageBumping()
        {
            while (_autoBumpIsOn)
            {
                await _bumper.SendBump();
                await Task.Delay(HowOften);
            }
        }
    }
}
