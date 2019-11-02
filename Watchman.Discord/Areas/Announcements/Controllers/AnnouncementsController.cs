using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Watchman.Discord.Framework.Architecture.Controllers;
using Watchman.Integrations.Disboard;

namespace Watchman.Discord.Areas.Announcements.Controllers
{
    class AnnouncementsController : ReadAlways, IController
    {
        private TimeSpan HowOften => new TimeSpan(2, 1, 0); // every 2 hours and 1 minute
        private CancellationTokenSource _tokenSource;
        private ServerBumper _bumper;

        [DiscordCommand("-autobump start")]
        public void AutoBumpStart(SocketMessage socketMessage)
        {
            this._tokenSource = new CancellationTokenSource();

            socketMessage.Channel.SendMessageAsync("Started auto bumping.");

            _bumper = new ServerBumper(socketMessage.Channel);
            _ = BumpUntilNotStopped();
        }

        [DiscordCommand("-autobump stop")]
        public void AutoBumpStop(SocketMessage socketMessage)
        {
            this._tokenSource.Cancel();
            socketMessage.Channel.SendMessageAsync("Stopped auto bumping.");
        }

        private async Task BumpUntilNotStopped()
        {
            var factory = new TaskFactory(_tokenSource.Token);

            while (true)
            {
                await factory.StartNew(() => _bumper.SendBump());
                await Task.Delay(HowOften);
            }
        }
    }
}
