using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NET.Popo
{
    public abstract class MessageHandler : PopoObject
    {
        public NetChannel Channel { get; set; }
        public MessageHandler(NetChannel channel, int handlerId)
        {
            this.Channel = channel;
            this.Channel.OnClose += () => { this.Close(); };
        }

        public async Task<Packet> CallRpc(Packet packet)
        {
            Packet response = null;
            var tcs = new TaskCompletionSource<Packet>();
            await Channel.RequestAsync(packet, (p) => { tcs.SetResult(response); });
            return await tcs.Task;
        }

        public async Task SendAsync(Packet packet)
        {
            await Channel.SendAsync(packet);
        }

        public abstract void OnReceive(Packet packet);
    }
}
