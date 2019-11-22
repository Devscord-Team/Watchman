using System;
using System.Collections.Generic;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Discord.WebSocket;
using Watchman.Integrations.Disboard;

namespace Watchman.Discord.Areas.Announcements.Controllers
{
    //class AnnouncementsController : IController
    //{
    //    private readonly ServerBumper _bumper;

    //    public AnnouncementsController()
    //    {
    //        _bumper = new ServerBumper();
    //    }

    //    [AdminCommand]
    //    [DiscordCommand("-autobump start")]
    //    public void AutoBumpStart(string message, Dictionary<string, IDiscordContext> contexts)
    //    {
    //        var channel = GetChannel(contexts);
    //        var isStarted = _bumper.AddServerChannel(channel);

    //        if (isStarted.Result)
    //        {
    //            channel.SendMessageAsync("Rozpoczęto automatyczne podbijanie.");
    //        }
    //    }

    //    [AdminCommand]
    //    [DiscordCommand("-autobump stop")]
    //    public void AutoBumpStop(string message, Dictionary<string, IDiscordContext> contexts)
    //    {
    //        var channel = GetChannel(contexts);
    //        var isStopped = _bumper.RemoveServerChannel(channel);

    //        if (isStopped.Result)
    //        {
    //            channel.SendMessageAsync("Zatrzymano automatyczne podbijanie.");
    //        }
    //    }
    //}
}
