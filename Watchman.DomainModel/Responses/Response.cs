using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Responses
{
    public class Response : Entity, IAggregateRoot
    {
        public const ulong DEFAULT_SERVER_ID = 0;
        public string OnEvent { get; private set; }
        public string Message { get; private set; }
        public ulong ServerId { get; private set; }
        public string[] AvailableVariables { get; private set; }
        public bool IsDefault => this.ServerId == DEFAULT_SERVER_ID;

        public Response(string onEvent, string message, ulong serverId, string[] availableVariables)
        {
            this.OnEvent = onEvent;
            this.Message = message;
            this.ServerId = serverId;
            this.AvailableVariables = availableVariables;
        }

        public void SetOnEvent(string onEvent)
        {
            if (onEvent == this.OnEvent)
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
