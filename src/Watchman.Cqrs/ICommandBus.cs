using System.Threading.Tasks;

namespace Watchman.Cqrs
{
    public interface ICommandBus
    {
        Task ExecuteAsync<T>(T command) where T : ICommand;
    }
}
