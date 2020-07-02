using Devscord.DiscordFramework;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.CustomCommands.Commands;
using Watchman.DomainModel.CustomCommands.Queries;

namespace Watchman.Discord.Integration.DevscordFramework
{
    public class CustomCommandsLoader : ICustomCommandsLoader
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

<<<<<<< HEAD
        public CustomCommandsLoader(IQueryBus queryBus)
        {
            this._queryBus = queryBus;
        }
=======
        public CustomCommandsLoader(IQueryBus queryBus, ICommandBus commandBus)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
        }
>>>>>>> master
        public async Task<List<CustomCommand>> GetCustomCommands()
        {
            var query = new GetCustomCommandsQuery();
            var commands = await this._queryBus.ExecuteAsync(query);
            var mapped = commands.CustomCommands.Select(x =>
            {
                var regex = new Regex(x.CustomTemplateRegex.Replace(@"\\", @"\"), RegexOptions.Compiled | RegexOptions.IgnoreCase);
                return new CustomCommand(x.CommandFullName, regex, x.ServerId);
            }).ToList();
            return mapped;
        }

        public async Task InitDefaultCustomCommands()
        {
            var customCommands = new List<AddCustomCommandsCommand>();
            foreach (var serverId in new ulong[] { 636238466899902504, 597066406521208852 })
            {
                customCommands.AddRange(new List<AddCustomCommandsCommand>
                {
                    new AddCustomCommandsCommand("Watchman.Discord.Areas.Users.BotCommands.AddRoleCommand", @"-add\s*role\s*(?<Roles>[\w\W\s\""]*)", serverId),
                    new AddCustomCommandsCommand("Watchman.Discord.Areas.Users.BotCommands.RemoveRoleCommand", @"-remove\s*role\s*(?<Roles>[\w\W\s\""]*)", serverId),
                    new AddCustomCommandsCommand("Watchman.Discord.Areas.Administration.BotCommands.SetRoleCommand", @"-set\s*role\s*(?<Roles>[\w\W\s\""]*)\s*-((?<Safe>safe)|(?<Unsafe>unsafe))", serverId),
                });
            }
            var commandsInBase = this._queryBus.Execute(new GetCustomCommandsQuery()).CustomCommands.ToList();
            foreach (var command in customCommands)
            {
                if (commandsInBase.Any(x => x.ServerId == command.ServerId && x.CommandFullName == command.CommandFullName))
                {
                    continue;
                }
                await this._commandBus.ExecuteAsync(command);
            }
        }
    }
}
