using CSSL.Modeling.CSSLQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Examples.ServerpoolsDispatcher
{
    public class Job : CSSLQueueObject
    {
        public Job(double creationTime) : base(creationTime)
        {
        }

        public double serviceTime { get; }
    }
}
