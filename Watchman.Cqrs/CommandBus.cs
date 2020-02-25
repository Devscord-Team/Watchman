using System;
using System.Threading.Tasks;
using Autofac;
using Serilog;

namespace Watchman.Cqrs
{
    public class CommandBus : ICommandBus
    {
        private readonly IComponentContext _context;

        public CommandBus(IComponentContext context)
        {
            _context = context;
        }

        public async Task ExecuteAsync<T>(T command) where T : ICommand
        {
            Log.Debug("Command: {command}", command);
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command),
                    $"Command: '{typeof(T).Name}' can not be null.");
            }
            var handler = _context.Resolve<ICommandHandler<T>>();

            await handler.HandleAsync(command);
        }
    }
}
