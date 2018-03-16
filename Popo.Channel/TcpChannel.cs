using System;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;
using System.Net;

namespace Popo.Channel
{
    public class TcpChannel : NetChannel
    {
        private SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1);
        private TcpClient tcpClient;
        private NetworkStream netStream;
        private DateTime recvTime = DateTime.Now;
        private ConcurrentDictionary<int, Action<Packet>> rpcActions = new ConcurrentDictionary<int, Action<Packet>>();       

        private int rpcId;
        private int newRpcId
        {
            get
            {
                Interlocked.Increment(ref rpcId);
                Interlocked.CompareExchange(ref rpcId, 1, int.MaxValue);
                return rpcId;
            }
        }

        public TcpChannel(TcpClient tcpClient, IPEndPoint endPoint) : this(tcpClient)
        {
            EndPoint = endPoint;
        }

        public TcpChannel(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            RecvParser = new PacketParse();
            SendParser = new PacketParse();
            netStream = tcpClient.GetStream();
            StartRecv();
        }

        public override async Task<bool> StartConnecting()
        {
            if (Connected)
                return true;
            try
            {
                CancellationTokenSource cancel = new CancellationTokenSource(3000);
                cancel.Token.Register(() => { if (Connected) { DisConnect(); } }, false);
                await tcpClient.ConnectAsync(EndPoint.Address, EndPoint.Port);
                if (CallConnect())
                {
                    Connected = true;
                    return true;
                }

            }
            catch(Exception e)
            {
                Console.Write(e.ToString());
            }
            return false;
        }

        public override async Task<bool> ReConnecting()
        {
            if (CallConnect() && Connected)
            {
                DisConnect();
            }
            tcpClient = new TcpClient();
            return await StartConnecting();
        }

        private bool CallConnect()
        {
            try
            {
                if (!tcpClient.Connected || tcpClient.Available <= 0 || tcpClient.Client.Available <= 0)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public override async Task SendAsync(Packet packet)
        {
            await SemaphoreSlim.WaitAsync();
            SendParser.WriteBuffer(packet);
            if (!netStream.CanWrite)
            {
                return;
            }
            while (SendParser.Buffer.DataSize > 0)
            {
                CancellationTokenSource cancel = new CancellationTokenSource(3000);
                cancel.Token.Register(() => { if (Connected) { DisConnect(); } }, false);
                await netStream.WriteAsync(SendParser.Buffer.First, SendParser.Buffer.FirstOffset, SendParser.Buffer.FirstCount, cancel.Token);
                SendParser.Buffer.UpdateRead(SendParser.Buffer.FirstCount);
            }
            SemaphoreSlim.Release();
        }

        public override async Task RequestAsync(Packet packet, Action<Packet> recvAction)
        {
            packet.IsRpc = true;
            packet.RpcId = newRpcId;
            rpcActions.AddOrUpdate(packet.RpcId, recvAction, (p, a) => { return a; });
            await SendAsync(packet);
        }

        private async void StartRecv()
        {
            while (true)
            {
                if (!netStream.CanRead)
                {
                    return;
                }

                try
                {
                    CancellationTokenSource cancel = new CancellationTokenSource(3000);
                    cancel.Token.Register(() =>{if (Connected){DisConnect();}}, false);
                    var count = await netStream.ReadAsync(RecvParser.Buffer.Last, RecvParser.Buffer.LastOffset, RecvParser.Buffer.LastCount, cancel.Token);
                    if (count <= 0)
                    {
                        throw new SocketException((int)SocketError.SocketError);
                    }
                    RecvParser.Buffer.UpdateWrite(count);
                    while (true)
                    {
                        var packet = RecvParser.ReadBuffer();
                        if (packet == null)
                        {
                            break;
                        }
                        if (!packet.IsHeartbeat)
                        {
                            if (packet.IsRpc)
                            {
                                if(rpcActions.TryRemove(packet.RpcId, out Action<Packet> action))
                                {
                                    action(packet);
                                }
                            }
                            else
                            {
                                OnReceive?.Invoke(packet);
                            }
                        }
                    }
                }
                catch
                {
                    OnError?.Invoke(this);
                }

            }
        }

        public override void DisConnect()
        {
            try
            {
                Connected = false;
                netStream.Close();
                netStream.Dispose();
            }
            catch{}

            try
            {
                tcpClient.Close();
                tcpClient.Dispose();
            }
            catch { }

            OnDisconnect?.Invoke();
        }
    }
}
