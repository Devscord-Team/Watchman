using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Responses
{
    public class Response : Entity, IAggregateRoot
    {
        public string OnEvent { get; private set; }
        public string Message { get; private set; }

        public Response(string onEvent, string message)
        {
            OnEvent = onEvent;
            Message = message;
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
