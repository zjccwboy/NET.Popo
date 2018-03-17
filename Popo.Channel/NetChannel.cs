using Popo.Object;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Popo.Channel
{
    public enum ChannelType
    {
        Server,
        Client,
    }

    public class NetTypeAttribute : Attribute
    {
        public ChannelType ChannelType { get; private set; }
        public NetTypeAttribute(ChannelType channelType)
        {
            ChannelType = channelType;
        }
    }

    public abstract class NetChannel : PopoObject
    {
        /// <summary>
        /// 通道类型
        /// </summary>
        public ChannelType ChannelType { get; set; }
        /// <summary>
        /// 接收包解析器
        /// </summary>
        protected PacketParse RecvParser;
        /// <summary>
        /// 发送包解析器
        /// </summary>
        protected PacketParse SendParser;
        /// <summary>
        /// 接收回调事件
        /// </summary>
        public Action<Packet> OnReceive;
        /// <summary>
        /// 错误回调事件
        /// </summary>
        public Action<NetChannel, SocketError> OnError;
        /// <summary>
        /// 通道关闭事件
        /// </summary>
        /// <returns></returns>
        public Action OnClose;
        /// <summary>
        /// 连接状态
        /// </summary>
        public bool Connected { get; protected set; }
        /// <summary>
        /// IP地址结构
        /// </summary>
        public IPEndPoint EndPoint { get; set; }
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
        /// 开始接收数据
        /// </summary>
        /// <returns></returns>
        public abstract Task StartRecv();
        /// <summary>
        /// 发送Rpc请求
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="recvAction"></param>
        /// <returns></returns>
        public abstract Task RequestAsync(Packet packet, Action<Packet> recvAction);
    }
}