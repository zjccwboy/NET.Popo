using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Popo.Object
{
    public class ObjectPool
    {
        private Dictionary<long, PopoObject> objects = new Dictionary<long, PopoObject>();
        private Dictionary<Type, Queue<PopoObject>> objectQueue = new Dictionary<Type, Queue<PopoObject>>();



    }
}
