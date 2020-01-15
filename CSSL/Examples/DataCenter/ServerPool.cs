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
    public class Serverpool : SchedulingElement
    {
        public Serverpool(ModelElement parent, string name) : base(parent, name)
        {
            queue = new ServerPoolQueue(parent, name + "_Queue");
        }

        private ServerPoolQueue queue { get; set; }

        public int JobCount { get; private set; }

        public void HandleDeparture(CSSLEvent e)
        {

        }

        public void HandleArrival(Job job)
        {
            queue.EnqueueAndSort(job);
        }

        public class ServerPoolQueue : CSSLQueue<Job>
        {
            public ServerPoolQueue(ModelElement parent, string name) : base(parent, name)
            {
            }

            /// <summary>
            /// Adds the job to the queue and sorts the queue on departure times.
            /// </summary>
            /// <param name="job">The job to be added.</param>
            public void EnqueueAndSort(Job job)
            {
                Items.Add(job);

                Items = Items.OrderBy(x => x.DepartureTime).ToList();
            }
        }
    }
}
