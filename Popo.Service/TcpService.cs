using Popo.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Popo.Service
{
    public class TcpService : NetService , IDisposable
    {
        private readonly List<Type> handlerTypes = new List<Type>();
        private TcpListener tcpListener;
        private IPEndPoint endPoint;
        public NetChannel CurrentChannel { get; private set; }
        
        public TcpService(IPEndPoint endPoint, Type[] handlerTypes)
        {
            this.handlerTypes.AddRange(handlerTypes);
            this.endPoint = endPoint;
            tcpListener = new TcpListener(endPoint);
            tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            tcpListener.Server.NoDelay = true;
            tcpListener.Start();
        }

        public override async Task AcceptAsync()
        {
            while (true)
            {
                var client = await tcpListener.AcceptTcpClientAsync();
                var channel = new TcpChannel(client);
                channel.OnError = (c,e)=> { Channels.Remove(c.ObjectId); };
                Channels.Add(channel.ObjectId, channel);
                channel.OnReceive = (p) => { OnReceive(channel, p); };
            }
        }

        public override async Task ConnectAsync()
        {
            if (CurrentChannel == null)
            {
                var tcpClient = new TcpClient();
                CurrentChannel = new TcpChannel(tcpClient, endPoint);
            }

            if (await CurrentChannel.StartConnecting())
            {
                CurrentChannel.OnError = (c,e) => { Channels.Remove(c.ObjectId); };
                Channels.Add(CurrentChannel.ObjectId, CurrentChannel);
                CurrentChannel.OnReceive = (p) => { OnReceive(CurrentChannel, p); };
            }
        }

        private void CreateChannelHandlers(NetChannel channel)
        {
            foreach(var type in handlerTypes)
            {
                MessageHandlerManager.Create(type, channel);
            }
        }

        public void Dispose()
        {
            var values = Channels.Values;
            foreach (var channel in values)
            {
                channel.DisConnect();
                Channels.Remove(channel.ObjectId);
            }
        }
    }
}
