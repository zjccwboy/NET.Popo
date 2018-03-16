using NET.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPacketParse
{
    class Program
    {
        static void Main(string[] args)
        {

            
            var parse = new PacketParse();
            
            long testCount = 0;
            long testLastCount = 0;
            while (true)
            {
                
                var randomLeng = new Random().Next(100, 1000);
                var testStr = string.Empty;
                for(int i=0;i<randomLeng; i++)
                {
                    testStr += i.ToString();
                }
                var testData = Encoding.Default.GetBytes(testStr);

                var random = new Random().Next(10, 10000);
                var writeCount = 0;
                for (int i = 0; i < random; i++)
                {
                    parse.WriteBuffer(new Packet() { Data = testData });
                    writeCount++;
                }
                //Console.WriteLine(writeCount);

                var readCount = 0;
                while (true)
                {
                    var packet = parse.ReadBuffer();
                    if(packet == null)
                    {
                        break;
                    }
                    if(testStr != Encoding.Default.GetString(packet.Data))
                    {
                        Console.WriteLine(Encoding.Default.GetString(packet.Data));
                    }
                    readCount++;
                }

                if(writeCount != readCount)
                {
                    Console.WriteLine($"write {writeCount} read {readCount}");
                }

                testCount++;
                if (testCount - testLastCount == 100)
                {
                    testLastCount = testCount;
                    Console.WriteLine($"TestCount {testCount}");
                }                
            }


        }
    }
}
