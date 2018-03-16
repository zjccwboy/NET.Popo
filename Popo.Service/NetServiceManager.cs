using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Popo.Object;

namespace Popo.Service
{
    public class NetServiceManager
    {
        public static T Create<T>(IPEndPoint iPEndPoint) where T : NetService
        {
            var type = typeof(T);
            var service = (T)PopoObjectPool.Fetch(typeof(T), iPEndPoint);
            return service;
        }

        public static NetService Create(Type type,IPEndPoint iPEndPoint)
        {
            var service = (NetService)PopoObjectPool.Fetch(type, iPEndPoint);
            return service;
        }
    }
}
