﻿using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework
{
    internal class ControllersContainer
    {
        public IReadOnlyList<ControllerInfo> WithReadAlways { get; private set; }
        public IReadOnlyList<ControllerInfo> WithDiscordCommand { get; private set; }
        public IReadOnlyList<ControllerInfo> WithIBotCommand { get; private set; }

        public ControllersContainer(IReadOnlyList<ControllerInfo> controllers)
        {
            this.WithReadAlways = controllers
                .Select(x => new ControllerInfo(x.Controller, x.Methods.Where(m => m.HasAttribute<ReadAlways>())))
                .Where(x => x.Methods.Any()).ToList();
            Log.Debug("Found {quantity} ReadAlwayd methods", this.WithReadAlways == null ? 0 : this.WithReadAlways.SelectMany(x => x.Methods).Count());

            this.WithDiscordCommand = controllers
                .Select(x => new ControllerInfo(x.Controller, x.Methods.Where(m => m.HasAttribute<DiscordCommand>())))
                .Where(x => x.Methods.Any()).ToList();
            Log.Debug("Found {quantity} DiscordCommand methods", this.WithDiscordCommand == null ? 0 : this.WithDiscordCommand.SelectMany(x => x.Methods).Count());

            this.WithIBotCommand = controllers
                .Select(x => new ControllerInfo(x.Controller, x.Methods.Where(x => x.HasParameter<IBotCommand>())))
                .Where(x => x.Methods.Any()).ToList();
            Log.Debug("Found {quantity} IBotCommand methods", this.WithIBotCommand == null ? 0 : this.WithIBotCommand.SelectMany(x => x.Methods).Count());
        }
    }
}
