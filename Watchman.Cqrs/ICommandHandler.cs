using System.Threading.Tasks;

namespace Watchman.Cqrs
{
    public interface ICommandHandler
    {
    }

    public interface ICommandHandler<in T> : ICommandHandler where T : ICommand
    {
        Task HandleAsync(T command);
    }
}
