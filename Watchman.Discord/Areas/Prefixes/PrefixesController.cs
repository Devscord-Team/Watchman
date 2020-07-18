using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Discord.Areas.Prefixes.Commands;

namespace Watchman.Discord.Areas.Prefixes
{
    public class PrefixesController : IController
    {
        public async Task GetPrefixes(PrefixesCommand command, Contexts contexts)
        {
        }

        public async Task AddPrefix(AddPrefixCommand command, Contexts contexts)
        {
        }

        public async Task RemovePrefix(RemovePrefixCommand command, Contexts contexts)
        {
        }
    }
}
