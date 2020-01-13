using CSSL.Modeling;
using CSSL.Modeling.CSSLQueue;
using CSSL.Modeling.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Examples.ServerpoolsDispatcher
{
    public class Dispatcher : SchedulingElement
    {
        public Dispatcher(ModelElement parent, string name) : base(parent, name)
        {
        }

        private Queue<CSSLQueueObject> queue;
    }
}
