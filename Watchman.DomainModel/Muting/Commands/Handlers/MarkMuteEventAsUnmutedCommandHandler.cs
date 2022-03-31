using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Muting;
using Watchman.DomainModel.Muting.Commands;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Muting.Commands.Handlers
{
    public class MarkMuteEventAsUnmutedCommandHandler : ICommandHandler<MarkMuteEventAsUnmutedCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public MarkMuteEventAsUnmutedCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(MarkMuteEventAsUnmutedCommand command)
        {
            using var session = this._sessionFactory.CreateMongo();
            var muteEvent = session.Get<MuteEvent>(command.MuteEventGuid);
            muteEvent.Unmute();
            await session.UpdateAsync(muteEvent);
        }
    }
}
