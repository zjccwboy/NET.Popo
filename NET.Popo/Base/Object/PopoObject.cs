using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET.Popo
{
    public abstract class PopoObject
    {
        public int ObjectId { get; set; }

        public virtual void Close()
        {
            PopoObjectPool.Push(this);
        }
    }
}
