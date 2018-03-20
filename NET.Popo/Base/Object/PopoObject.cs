using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET.Popo
{
    public abstract class PopoObject
    {
        public long ObjectId { get; set; }

        public virtual void Close()
        {
            PopoObjectPool.Push(this);
        }
    }
}
