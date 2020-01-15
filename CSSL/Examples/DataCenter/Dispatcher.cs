using CSSL.Modeling;
using CSSL.Modeling.CSSLQueue;
using CSSL.Modeling.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Examples.DataCenter
{
    public class Dispatcher : SchedulingElement
    {
        public Dispatcher(ModelElement parent, string name) : base(parent, name)
        {
            queue = new CSSLQueue<CSSLQueueObject>(parent, name + "_Queue");
        }

        private CSSLQueue<CSSLQueueObject> queue;

        public void HandleArrival(CSSLEvent e)
        {
            if(queue.Length == 0)
            {

            }
        }
    }
}
