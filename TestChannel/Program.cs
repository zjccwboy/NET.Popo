using Popo.Channel;
using Popo.Object;
using Popo.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestChannel
{
    class Program
    {
        static TcpService service;
        static void Main(string[] args)
        {
            PopoObjectPool.Load();

            //IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9936);
            //service = new TcpService(iPEndPoint);
            //service.Start();

            //var count = 0;
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            
            //service.OnReceive = async(c, p) =>
            //{
            //    try
            //    {
            //        await c.SendAsync(p);
            //        Interlocked.Increment(ref count);
            //       // Console.WriteLine($"{p.RpcId}");
            //    }
            //    catch(Exception e)
            //    {
            //        Console.WriteLine(e.Message);
            //        await Task.Delay(10000000);
            //    }
            //};

            //Console.Read();
        }
    }
}
