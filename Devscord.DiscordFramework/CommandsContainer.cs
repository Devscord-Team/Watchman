using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework
{
    public class CommandsContainer : ICommandsContainer
    {
        private readonly ICustomCommandsLoader _customCommandsLoader;
        private Dictionary<ulong, List<CustomCommand>> _customCommandsGroupedByBotCommand;
        private DateTime _lastRefresh;

        public CommandsContainer(ICustomCommandsLoader customCommandsLoader)
        {
            this._customCommandsLoader = customCommandsLoader;
        }

        public async Task<CustomCommand> GetCommand(DiscordRequest request, Type botCommand, Contexts contexts)
        {
            await this.TryRefresh();
            if (!this._customCommandsGroupedByBotCommand.ContainsKey(contexts.Server.Id))
            {
                return null;
            }
            var serverCommands = this._customCommandsGroupedByBotCommand[contexts.Server.Id];
            try
            {
                var command = serverCommands.SingleOrDefault(x => x.ExpectedBotCommandName == botCommand.FullName && x.Template.IsMatch(request.OriginalMessage));
                return command;
            }
            catch (InvalidOperationException)
            {
                throw new MoreThanOneRegexHasBeenMatchedException();
            }   
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
