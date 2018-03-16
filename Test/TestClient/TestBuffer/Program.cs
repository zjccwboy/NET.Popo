using NET.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBuffer
{
    class Program
    {
        static void Main(string[] args)
        {

            NET.Channel.Buffer buffer = new NET.Channel.Buffer();

            var write = Encoding.Default.GetBytes("11111111111afafasfasssssssss11111werewfwefwefwefwefw11111");
            Packet packet = new Packet();
            packet.Data = write;

            PacketParse parse = new PacketParse();

            for (int i=0;i< 10000; i++)
            {
                parse.WriteBuffer(packet);
            }

            var count = 0;
            while (true)
            {
                List<Packet> packets = parse.ReadBuffer();
                if (packets != null)
                {
                    foreach(var p in packets)
                    {
                        count++;
                        Console.WriteLine($"count {count} data{Encoding.Default.GetString(p.Data)}");
                    }                    
                }
            }


            Console.Read();
        }
    }
}
