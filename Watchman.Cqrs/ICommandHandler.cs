using System.Threading.Tasks;
using Watchman.Cqrs;

namespace Watchman.Cqrs
{
    public interface ICommandHandler
    {
    }

    public interface ICommandHandler<T> : ICommandHandler where T : ICommand
    {
        Task HandleAsync(T command);
    }
}
