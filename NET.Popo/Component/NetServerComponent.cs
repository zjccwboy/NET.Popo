using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NET.Popo
{
    public class NetServerComponent : Component
    {
        private NetService netService;

        public NetServerComponent(IPEndPoint endPoint)
        {
            netService = new TcpService(endPoint);
            
        }

        public void AddMeesageHandler(MessageHandler handler)
        {
            netService.AddHandlerType(handler);
        }



    }
}
