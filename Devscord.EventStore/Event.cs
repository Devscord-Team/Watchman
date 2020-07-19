using System.Threading.Tasks;

namespace Devscord.EventStore
{
    public abstract class Event
    {
        public async Task Publish()
        {
            await EventStore.Publish(this);
        }
    }
}
