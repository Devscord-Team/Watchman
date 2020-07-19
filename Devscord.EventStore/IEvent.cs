namespace Devscord.EventStore
{
    public interface IEvent
    {
        void Publish()
        {
            EventStore.Publish(this);
        }
    }
}
