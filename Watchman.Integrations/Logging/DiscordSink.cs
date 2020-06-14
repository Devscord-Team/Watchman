using Serilog.Core;
using Serilog.Events;
using System;

namespace Watchman.Integrations.Logging
{
    public class DiscordSink : ILogEventSink
    {
        private readonly Action<string> _onEmit;

        public DiscordSink(Action<string> onEmit = null)
        {
            _onEmit = onEmit;
        }

        public void Emit(LogEvent logEvent)
        {
            if(_onEmit == null)
            {
                return;
            }

            var message = logEvent.RenderMessage();
            _onEmit.Invoke(message);
        }
    }
}
