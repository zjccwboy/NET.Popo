using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NET.Popo
{
    public abstract class NetService : PopoObject, IDisposable
    {
        protected readonly ConcurrentDictionary<long, NetChannel> Channels = new ConcurrentDictionary<long, NetChannel>();
        public abstract Task AcceptAsync();
        public abstract Task ConnectAsync();
        public abstract void AddHandlerType(Type handlerType);
        public virtual void Dispose()
        {

        }
    }

}
