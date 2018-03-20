using System;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;
using System.Net;

namespace NET.Popo
{
    public class TcpChannel : NetChannel
    {
        public TcpClient TcpClient { get; set; }

        private SemaphoreSlim sendSemaphore = new SemaphoreSlim(1);        
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

        public TcpChannel(IPEndPoint endPoint):base()
        {
            EndPoint = endPoint;

        }
        private TcpChannel()
        {
            RecvParser = new PacketParse();
            SendParser = new PacketParse();
        }

        public override async Task<bool> StartConnecting()
        {
            if (Connected)
                return true;
            try
            {
                CancellationTokenSource cancel = new CancellationTokenSource(3000);
                cancel.Token.Register(() => { if (Connected) { OnError?.Invoke(this, SocketError.SocketError); } }, false);
                await TcpClient.ConnectAsync(EndPoint.Address, EndPoint.Port);
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
            int retry = 0;
            while (true)
            {
                if (!CallConnect() && Connected)
                {
                    DisConnect();
                    TcpClient = new TcpClient();
                    netStream = TcpClient.GetStream();
                }
                var isSuccess = await StartConnecting();
                if (isSuccess)
                {
                    return true;
                }
                retry++;
                if(retry == 5)
                {
                    return false;
                }
            }
        }

        private bool CallConnect()
        {
            try
            {
                if (!TcpClient.Connected || TcpClient.Available <= 0 || TcpClient.Client.Available <= 0)
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
            await sendSemaphore.WaitAsync();
            SendParser.WriteBuffer(packet);
            if (!netStream.CanWrite)
            {
                return;
            }
            while (SendParser.Buffer.DataSize > 0)
            {
                CancellationTokenSource cancel = new CancellationTokenSource(3000);
                cancel.Token.Register(() => { if (Connected) { OnError?.Invoke(this, SocketError.SocketError); } }, false);
                await netStream.WriteAsync(SendParser.Buffer.First, SendParser.Buffer.FirstOffset, SendParser.Buffer.FirstCount, cancel.Token);
                SendParser.Buffer.UpdateRead(SendParser.Buffer.FirstCount);
            }
            sendSemaphore.Release();
        }

        public override async Task RequestAsync(Packet packet, Action<Packet> recvAction)
        {
            packet.IsRpc = true;
            packet.RpcId = newRpcId;
            rpcActions.AddOrUpdate(packet.RpcId, recvAction, (p, a) => { return a; });
            await SendAsync(packet);
        }

        public override async Task StartRecv()
        {
            while (true)
            {
                if (!netStream.CanRead)
                {
                    return;
                }

                try
                {
                    var count = await netStream.ReadAsync(RecvParser.Buffer.Last, RecvParser.Buffer.LastOffset, RecvParser.Buffer.LastCount);
                    if (count <= 0)
                    {
                        throw new SocketException((int)SocketError.SocketError);
                    }
                    RecvParser.Buffer.UpdateWrite(count);
                    recvTime = DateTime.Now;
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
                catch(SocketException e)
                {
                    OnError?.Invoke(this, e.SocketErrorCode);
                }
                catch(Exception e)
                {
                    Console.Write(e.ToString());
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
                TcpClient.Close();
                TcpClient.Dispose();
            }
            catch { }
        }

        public override void Close()
        {            
            DisConnect();
            rpcActions.Clear();
            SendParser.Clear();
            RecvParser.Clear();
            OnClose?.Invoke();
            OnClose = null;
            OnError = null;
            OnReceive = null;
            base.Close();
        }
    }
}
