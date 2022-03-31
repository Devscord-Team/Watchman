using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Architecture.Controllers;
using Devscord.DiscordFramework.Commands;
using Serilog;
using System.Linq;

namespace Devscord.DiscordFramework.Routing.Models
{
    internal class ControllersContainer
    {
        public ControllerInfo[] WithReadAlways { get; private set; }
        public ControllerInfo[] WithDiscordCommand { get; private set; }
        public ControllerInfo[] WithIBotCommand { get; private set; }

        public ControllersContainer(ControllerInfo[] controllers)
        {
            this.WithReadAlways = controllers
                .Select(x => new ControllerInfo(x.Controller, x.Methods.Where(m => m.HasAttribute<ReadAlways>()).ToArray()))
                .Where(x => x.Methods.Any()).ToArray();
            Log.Debug("Found {quantityOfFoundCommandMethods} {commandsType} methods",
                this.WithReadAlways == null ? 0 : this.WithReadAlways.SelectMany(x => x.Methods).Count(),
                "ReadAlways");

            this.WithDiscordCommand = controllers
                .Select(x => new ControllerInfo(x.Controller, x.Methods.Where(m => m.HasAttribute<DiscordCommand>()).ToArray()))
                .Where(x => x.Methods.Any()).ToArray();
            Log.Debug("Found {quantityOfFoundCommandMethods} {commandsType} methods",
                this.WithDiscordCommand == null ? 0 : this.WithDiscordCommand.SelectMany(x => x.Methods).Count(),
                "DiscordCommand");

            this.WithIBotCommand = controllers
                .Select(x => new ControllerInfo(x.Controller, x.Methods.Where(x => x.HasParameter<IBotCommand>()).ToArray()))
                .Where(x => x.Methods.Any()).ToArray();
            Log.Debug("Found {quantityOfFoundCommandMethods} {commandsType} methods",
                this.WithIBotCommand == null ? 0 : this.WithIBotCommand.SelectMany(x => x.Methods).Count(),
                "IBotCommand");
        }
    }
}
