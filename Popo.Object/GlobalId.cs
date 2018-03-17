using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Popo.Object
{
    public class GlobalId
    {
        private static Random random = new Random();

        public static long CreateId()
        {
            var tks = DateTime.Now.Ticks;
            return tks | (long)random.Next(1, 100) << 47;
        }
    }
}
