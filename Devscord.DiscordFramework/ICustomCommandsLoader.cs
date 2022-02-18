using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework
{
    public interface ICustomCommandsLoader
    {
        Task<List<CustomCommand>> GetCustomCommands();
        Task InitDefaultCustomCommands();
    }
}
