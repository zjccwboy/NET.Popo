using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace NET.Channel
{
    public abstract class NetChannel
    {
        protected PacketParse RecvParser;
        protected PacketParse SendParser;
        public Action<Packet> OnReceive;
        public Action<NetChannel> OnError;
        public bool Connected { get; protected set; }
        public IPEndPoint EndPoint { get; protected set; }
        public abstract Task<bool> StartConnecting();
        public abstract Task<bool> ReConnecting();
        public abstract void DisConnect();
        public long ChannelId { get;protected set; }
        public abstract Task SendAsync(Packet packet);
        public abstract Task RequestAsync(Packet packet, Action<Packet> recvAction);
        
    }
}