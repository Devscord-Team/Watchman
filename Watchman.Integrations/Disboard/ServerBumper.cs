//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Discord.WebSocket;

//namespace Watchman.Integrations.Disboard
//{
//    public class ServerBumper
//    {
//        private readonly List<BumpedChannel> _channels;

//        public ServerBumper()
//        {
//            _channels = new List<BumpedChannel>();
//        }

//        public Task<bool> AddServerChannel(ISocketMessageChannel channel)
//        {
//            if (_channels.Any(x => x.MessageChannel == channel))
//            {
//                channel.SendMessageAsync("Na tym kanale już trwa podbijanie, nie możesz uruchomić kolejnego procesu podbijania na tym samym kanale.");
//                return Task.FromResult(false);
//            }

//            var bumpedChannel = new BumpedChannel(channel);
//            bumpedChannel.BumpServer();
//            this._channels.Add(bumpedChannel);
//            return Task.FromResult(true);
//        }

//        public Task<bool> RemoveServerChannel(ISocketMessageChannel channel)
//        {
//            var bumpedChannel = _channels.Find(x => x.MessageChannel == channel);

//            if (bumpedChannel == null)
//            {
//                channel.SendMessageAsync("Na tym kanale nie ma żadnego aktywnego podbijania.");
//                return Task.FromResult(false);
//            }

//            bumpedChannel.CancelBumping();
//            _channels.Remove(bumpedChannel);
//            return Task.FromResult(true);
//        }
//    }
//}
