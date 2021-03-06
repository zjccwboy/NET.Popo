﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NET.Popo
{
    public class TcpService : NetService
    {
        private readonly HashSet<Type> handlerTypes = new HashSet<Type>();
        private TcpListener tcpListener;
        private IPEndPoint endPoint;

        public TcpService(IPEndPoint endPoint)
        {
            this.endPoint = endPoint;
            tcpListener = new TcpListener(endPoint);
            tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            tcpListener.Server.NoDelay = true;
            tcpListener.Start();
        }

        public override void AddHandlerType(Type handlerType)
        {
            if (!handlerTypes.Contains(handlerType))
            {
                handlerTypes.Add(handlerType);
                foreach(var channel in Channels.Values)
                {
                    MessageHandlerFactory.Create(handlerType, channel);
                }
            }
        }

        public override async Task AcceptAsync()
        {
            while (true)
            {
                var tcpClient = await tcpListener.AcceptTcpClientAsync();
                var channel = (TcpChannel)PopoObjectPool.Fetch(typeof(TcpChannel), endPoint);
                channel.ChannelType = ChannelType.Server;
                channel.TcpClient = tcpClient;
                channel.OnError = OnChannelError;
                Channels.TryAdd(channel.ObjectId, channel);
                CreateMessageHandlers(channel);
            }
        }

        public override async Task ConnectAsync()
        {
            var tcpClient = new TcpClient();
            var channel = (TcpChannel)PopoObjectPool.Fetch(typeof(TcpChannel), endPoint);
            channel.ChannelType = ChannelType.Client;
            channel.TcpClient = tcpClient;
            channel.EndPoint = endPoint;
            if (await channel.StartConnecting())
            {
                channel.OnError = OnChannelError;
                Channels.TryAdd(channel.ObjectId, channel);
                CreateMessageHandlers(channel);
            }
        }


        private async void OnChannelError(NetChannel channel, SocketError socketError)
        {
            if(channel.ChannelType == ChannelType.Client)
            {
                if (! await channel.ReConnecting())
                {
                    channel.Close();
                }
                else
                {
                    return;
                }
            }
            else
            {
                channel.DisConnect();
            }
            Channels.TryRemove(channel.ObjectId, out NetChannel valu);
        }

        private void CreateMessageHandlers(NetChannel channel)
        {
            foreach(var type in handlerTypes)
            {
                MessageHandlerFactory.Create(type, channel);
            }
        }

        public override void Dispose()
        {
            var values = Channels.Values;
            foreach (var channel in values)
            {
                channel.Close();
            }
            Channels.Clear();
        }

        public override void Close()
        {
            Dispose();
            base.Close();
        }
    }
}
