//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Discord.WebSocket;

//namespace Watchman.Integrations.Disboard
//{
//    class BumpedChannel
//    {
//        public ISocketMessageChannel MessageChannel { get; }
        
//        private const string _bumpMessage = "!d bump";

//        private static TimeSpan HowOften => new TimeSpan(2, 1, 0); // every 2 hours and 1 minute

//        private readonly CancellationTokenSource _cancellationToken;

//        public BumpedChannel(ISocketMessageChannel channel)
//        {
//            MessageChannel = channel;
//            _cancellationToken = new CancellationTokenSource();
//        }

//        public async Task BumpServer()
//        {
//            var factory = new TaskFactory(_cancellationToken.Token);

//            await Task.Delay(1500); // start bumping AFTER sending a message: "Started autobumping"
//            while (!factory.CancellationToken.IsCancellationRequested)
//            {
//                await SendBump();
//                await Task.Delay(HowOften);
//            }
//        }

//        public Task CancelBumping()
//        {
//            _cancellationToken.Cancel();
//            return Task.CompletedTask;
//        }

//        private Task SendBump()
//        {
//            MessageChannel.SendMessageAsync(_bumpMessage);
//            return Task.CompletedTask;
//        }
//    }
//}
