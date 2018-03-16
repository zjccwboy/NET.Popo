using Popo.Object;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Popo.Channel
{
    public abstract class NetChannel : PopoObject
    {
        /// <summary>
        /// 接收包解析器
        /// </summary>
        protected PacketParse RecvParser;
        /// <summary>
        /// 发送包解析器
        /// </summary>
        protected PacketParse SendParser;
        /// <summary>
        /// 接收回调
        /// </summary>
        public Action<Packet> OnReceive;
        /// <summary>
        /// 连接断开回调
        /// </summary>
        public Action OnDisconnect;
        /// <summary>
        /// 错误回调
        /// </summary>
        public Action<NetChannel, SocketError> OnError;
        /// <summary>
        /// 连接状态
        /// </summary>
        public bool Connected { get; protected set; }
        /// <summary>
        /// IP地址结构
        /// </summary>
        public IPEndPoint EndPoint { get; protected set; }
        /// <summary>
        /// 开始连接
        /// </summary>
        /// <returns></returns>
        public abstract Task<bool> StartConnecting();
        /// <summary>
        /// 重新连接
        /// </summary>
        /// <returns></returns>
        public abstract Task<bool> ReConnecting();
        /// <summary>
        /// 断开连接
        /// </summary>
        public abstract void DisConnect();
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public abstract Task SendAsync(Packet packet);
        /// <summary>
        /// 发送Rpc请求
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="recvAction"></param>
        /// <returns></returns>
        public abstract Task RequestAsync(Packet packet, Action<Packet> recvAction);
        
    }
}