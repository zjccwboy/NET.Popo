﻿using NET.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NET.Service
{
    public abstract class ChannelHandler
    {
        public int HandlerId { get; private set; }

        private NetChannel channel;
        public ChannelHandler(NetChannel channel, int handlerId)
        {
            this.channel = channel;
        }

        public async Task<Packet> CallRpc(Packet packet)
        {
            Packet response = null;
            var tcs = new TaskCompletionSource<Packet>();
            await channel.RequestAsync(packet, (p) => { tcs.SetResult(response); });
            return await tcs.Task;
        }

        public async Task SendAsync(Packet packet)
        {
            await channel.SendAsync(packet);
        }

        public abstract void OnReceive(Packet packet);

        public abstract void OnError();
    }
}
