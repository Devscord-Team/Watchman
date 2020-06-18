using Devscord.DiscordFramework;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.CustomCommands.Queries;

namespace Watchman.Discord.Integration.DevscordFramework
{
    public class CustomCommandsLoader : ICustomCommandsLoader
    {
        private readonly IQueryBus _queryBus;

        public CustomCommandsLoader(IQueryBus queryBus) => this._queryBus = queryBus;

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
    }
}
