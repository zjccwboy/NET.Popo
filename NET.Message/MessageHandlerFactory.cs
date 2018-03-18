using Popo.Channel;
using Popo.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NET.Message
{
    public class MessageHandlerFactory
    {

        public static T Create<T>(NetChannel channel) where T : MessageHandler
        {
            var type = typeof(T);
            var handler = (T)PopoObjectPool.Fetch(typeof(T), channel);
            return handler;
        }

        public static MessageHandler Create(Type handlerType, NetChannel channel)
        {
            var handler = (MessageHandler)PopoObjectPool.Fetch(handlerType, channel);
            return handler;
        }

    }
}
