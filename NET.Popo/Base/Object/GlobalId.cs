using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NET.Popo
{
    public class GlobalId
    {
        private static int assist;

        public static int CreateId()
        {
            Interlocked.Increment(ref assist);
            Interlocked.CompareExchange(ref assist, 1, int.MaxValue);
            return assist;
        }
    }
}
