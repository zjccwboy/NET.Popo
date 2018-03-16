using NET.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NET.Service
{
    public class ChannelHandlerManager
    {
        private static int handerId;
        private static int newHanderId
        {
            get
            {
                Interlocked.Increment(ref handerId);
                Interlocked.CompareExchange(ref handerId, 1, int.MaxValue);
                return handerId;
            }
        }

        public static T Create<T>(NetChannel channel) where T : ChannelHandler
        {
            var type = typeof(T);
            var handler = (T)Activator.CreateInstance(type, channel, newHanderId);
            return handler;
        }

        public static ChannelHandler Create(Type handlerType, NetChannel channel)
        {
            var handler = (ChannelHandler)Activator.CreateInstance(handlerType, channel, newHanderId);
            return handler;
        }

    }
}
