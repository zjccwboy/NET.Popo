using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NET.Service
{
    public class NetServiceManager
    {
        public static T Create<T>(IPEndPoint iPEndPoint) where T : NetService
        {
            var type = typeof(T);
            var service = (T)Activator.CreateInstance(type, iPEndPoint);
            return service;
        }


    }
}
