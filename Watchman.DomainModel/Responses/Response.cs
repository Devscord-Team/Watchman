using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Responses
{
    public class Response : Entity, IAggregateRoot
    {
        public const ulong DEFAULT_SERVER_ID = 0;
        public string OnEvent { get; private set; }
        public string Message { get; private set; }
        public ulong ServerId { get; private set; }
        public bool IsDefault => ServerId == DEFAULT_SERVER_ID;

        public Response(string onEvent, string message, ulong serverId)
        {
            OnEvent = onEvent;
            Message = message;
            ServerId = serverId;
        }

        public void SetOnEvent(string onEvent)
        {
            if(onEvent == this.OnEvent)
            {
                return;
            }
            this.OnEvent = onEvent;
            this.Update();
        }

        public void SetMessage(string message)
        {
            if (message == this.Message)
            {
                return;
            }
            this.Message = message;
            this.Update();
        }
    }
}
