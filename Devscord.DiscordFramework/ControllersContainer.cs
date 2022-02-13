using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Architecture.Controllers;
using Devscord.DiscordFramework.Commands;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework
{
    internal class ControllersContainer
    {
        public ControllerInfo[] WithReadAlways { get; private set; }
        public ControllerInfo[] WithDiscordCommand { get; private set; }
        public ControllerInfo[] WithIBotCommand { get; private set; }

        public ControllersContainer(ControllerInfo[] controllers)
        {
            this.WithReadAlways = controllers
                .Select(x => new ControllerInfo(x.Controller, x.Methods.Where(m => m.HasAttribute<ReadAlways>())))
                .Where(x => x.Methods.Any()).ToArray();
            Log.Debug("Found {quantity} ReadAlwayd methods", this.WithReadAlways == null ? 0 : this.WithReadAlways.SelectMany(x => x.Methods).Count());

            this.WithDiscordCommand = controllers
                .Select(x => new ControllerInfo(x.Controller, x.Methods.Where(m => m.HasAttribute<DiscordCommand>())))
                .Where(x => x.Methods.Any()).ToArray();
            Log.Debug("Found {quantity} DiscordCommand methods", this.WithDiscordCommand == null ? 0 : this.WithDiscordCommand.SelectMany(x => x.Methods).Count());

            this.WithIBotCommand = controllers
                .Select(x => new ControllerInfo(x.Controller, x.Methods.Where(x => x.HasParameter<IBotCommand>())))
                .Where(x => x.Methods.Any()).ToArray();
            Log.Debug("Found {quantity} IBotCommand methods", this.WithIBotCommand == null ? 0 : this.WithIBotCommand.SelectMany(x => x.Methods).Count());
        }
    }
}
