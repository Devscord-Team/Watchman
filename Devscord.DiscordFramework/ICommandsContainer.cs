using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework
{
    public interface ICommandsContainer
    {
        Task<CustomCommand> GetCommand(DiscordRequest request, Type botCommand, Contexts contexts);
    }
}
