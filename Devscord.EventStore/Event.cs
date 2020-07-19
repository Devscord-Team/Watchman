using System.Threading.Tasks;

namespace Devscord.EventStore
{
    public abstract class Event
    {
        public abstract string Name { get; }

        public async Task Publish()
        {
            await EventStore.Publish(this.Name, this);
        }
    }
}
