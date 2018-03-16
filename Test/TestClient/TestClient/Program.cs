using NET.Channel;
using NET.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        static TcpService service;
        static void Main(string[] args)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9936);
            service = new TcpService(iPEndPoint, NetType.Client);
            service.Start();

            var count = 0;
            var data = new Packet()
            {
                Data = Encoding.Default.GetBytes(count.ToString())
            };
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            service.OnReceive = (c, p) => 
            {
                count++;
                data.Data = Encoding.Default.GetBytes(count.ToString());
                service.NetChannel.SendAsync(data);
                //Console.WriteLine($"OnReceive {Encoding.Default.GetString(p.Data)}");
                if(stopWatch.ElapsedMilliseconds > 1000)
                {
                    Console.WriteLine(count);
                    count = 0;
                    stopWatch.Restart();
                }
            };

            Thread.Sleep(4000);
            service.NetChannel.SendAsync(data);
            Console.Read();
        }


    }
}
