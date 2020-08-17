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

        public CustomCommandsLoader(IQueryBus queryBus, ICommandBus commandBus)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
        }

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
                    new AddCustomCommandsCommand("Watchman.Discord.Areas.UselessFeatures.BotCommands.GoogleCommand", @"-google\s*(?<Search>.*)", serverId),
                    new AddCustomCommandsCommand("Watchman.Discord.Areas.Users.BotCommands.AddRoleCommand", @"-add\s*role\s+(?<Roles>.*)", serverId),
                    new AddCustomCommandsCommand("Watchman.Discord.Areas.Users.BotCommands.RemoveRoleCommand", @"-remove\s*role\s+(?<Roles>.*)", serverId),
                    new AddCustomCommandsCommand("Watchman.Discord.Areas.Administration.BotCommands.SetRoleCommand", @"-set\s*role\s+(?<Roles>.*)\s*-((?<Safe>safe)|(?<Unsafe>unsafe))", serverId),
                    new AddCustomCommandsCommand("Watchman.Discord.Areas.Users.BotCommands.Warns.AddWarnCommand", @"-add\s*warn\s+(?<User><\D{1,2}\d+>)\s+(?<Reason>.+)", serverId),
                    new AddCustomCommandsCommand("Watchman.Discord.Areas.Users.BotCommands.Warns.WarnsCommand", @"-warns\s*(?<User><\D{1,2}\d+>)?", serverId),
                    new AddCustomCommandsCommand("Watchman.Discord.Areas.Messaging.BotCommands.SendCommand", @"-send\s*(?<Channel>\<.*\>)\s*""?(?<Message>.*)\""?", serverId),
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
