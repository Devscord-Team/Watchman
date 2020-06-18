using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework
{
    public class CommandsContainer
    {
        private readonly ICustomCommandsLoader _customCommandsLoader;
        private Dictionary<ulong, List<CustomCommand>> _customCommandsGroupedByBotCommand;
        private DateTime _lastRefresh;

        public CommandsContainer(ICustomCommandsLoader customCommandsLoader) => this._customCommandsLoader = customCommandsLoader;

        public async Task<CustomCommand> GetCommand(DiscordRequest request, Type botCommand, ulong serverId)
        {
            await this.TryRefresh();
            if (!this._customCommandsGroupedByBotCommand.ContainsKey(serverId))
            {
                return null;
            }
            var serverCommands = this._customCommandsGroupedByBotCommand[serverId];
            var command = serverCommands.FirstOrDefault(x => x.ExpectedBotCommandName == botCommand.FullName && x.Template.IsMatch(request.OriginalMessage));
            return command;
        }

        private async Task TryRefresh()
        {
            if (this._lastRefresh > DateTime.UtcNow.AddMinutes(-15))
            {
                return;
            }
            var customCommands = await this._customCommandsLoader.GetCustomCommands();
            this._customCommandsGroupedByBotCommand = customCommands.GroupBy(x => x.ServerId).ToDictionary(k => k.Key, v => v.ToList());
            this._lastRefresh = DateTime.UtcNow;
        }
    }
}
