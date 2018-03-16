using Popo.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Popo.Service
{
    public enum NetType
    {
        Server,
        Client,
    }

    public class NetTypeAttribute : Attribute
    {
        public NetType NetType { get;private set; }
        public NetTypeAttribute(NetType netType)
        {
            NetType = netType;
        }
    }

    public abstract class NetService
    {
        protected readonly Dictionary<long, NetChannel> Channels = new Dictionary<long, NetChannel>();
        public Action<NetChannel,Packet> OnReceive;
        public abstract Task AcceptAsync();
        public abstract Task ConnectAsync();
    }

}
