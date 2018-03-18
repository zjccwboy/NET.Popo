using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Popo.Channel;
using Popo.Service;

namespace NET.Message
{
    public class MessageDistribute : MessageHandler
    {
        public MessageDistribute(NetChannel channel, int handlerId):base(channel, handlerId)
        {

        }

        public override void OnReceive(Packet packet)
        {
            throw new NotImplementedException();
        }
    }
}