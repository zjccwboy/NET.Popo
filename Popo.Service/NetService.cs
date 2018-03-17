using Popo.Channel;
using Popo.Object;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Popo.Service
{
    public abstract class NetService : PopoObject, IDisposable
    {
        protected readonly ConcurrentDictionary<long, NetChannel> Channels = new ConcurrentDictionary<long, NetChannel>();
        public abstract Task AcceptAsync();
        public abstract Task ConnectAsync();

        public virtual void Dispose()
        {

        }
    }

}
