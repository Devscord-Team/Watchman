using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Watchman.Discord.Framework;
using Watchman.Discord.Framework.Architecture.Controllers;
using Watchman.Discord.Framework.Architecture.Middlewares;
using Watchman.Discord.Middlewares.Contexts;
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
            var channel = GetChannel(contexts);
            var isStarted = _bumper.AddServerChannel(channel);

            if (isStarted.Result)
            {
                channel.SendMessageAsync("Rozpoczęto automatyczne podbijanie.");
            }
        }

        [AdminCommand]
        [DiscordCommand("-autobump stop")]
        public void AutoBumpStop(string message, Dictionary<string, IDiscordContext> contexts)
        {
            var channel = GetChannel(contexts);
            var isStopped = _bumper.RemoveServerChannel(channel);

            if (isStopped.Result)
            {
                channel.SendMessageAsync("Zatrzymano automatyczne podbijanie.");
            }
        }

        private ISocketMessageChannel GetChannel(Dictionary<string, IDiscordContext> contexts)
        {
            var channel = ((ChannelContext) contexts[nameof(ChannelContext)]);
            return (ISocketMessageChannel)Server.GetChannel(channel.Id);
        }
    }
}
