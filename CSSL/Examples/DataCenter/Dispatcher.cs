using CSSL.Modeling;
using CSSL.Modeling.CSSLQueue;
using CSSL.Modeling.Elements;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Examples.DataCenter
{
    public class Dispatcher : SchedulingElement
    {
        public Dispatcher(ModelElement parent, string name, Distribution serviceTimeDistribution, double serviceTimeThreshold, List<Serverpool> serverPools, int nrServerPools) : base(parent, name)
        {
            queue = new CSSLQueue<CSSLQueueObject>(parent, name + "_Queue");
            this.serviceTimeDistribution = serviceTimeDistribution;
            this.serviceTimeThreshold = serviceTimeThreshold;
            rnd = new Random();
            this.serverPools = serverPools;
            this.nrServerPools = nrServerPools;
        }

        private CSSLQueue<CSSLQueueObject> queue;

        private Distribution serviceTimeDistribution;

        private double serviceTimeThreshold;

        private Random rnd;

        private List<Serverpool> serverPools;

        private int nrServerPools;

        public void HandleArrival(Job job)
        {
            queue.EnqueueLast(job);

            if (queue.Length == 1)
            {
                ScheduleEvent(GetTime(), Dispatch);
            }
        }

        public void Dispatch(CSSLEvent e)
        {
            Job job = (Job)queue.DequeueFirst();

            double serviceTime = serviceTimeDistribution.Next();

            if (serviceTime > serviceTimeThreshold)
            {
                double serviceTime1 = serviceTime * rnd.NextDouble();
                double serviceTime2 = serviceTime - serviceTime1;

                Serverpool serverPool = ChooseServerpool();

                ScheduleEvent(GetTime() + serviceTime1, serverPool.)
            }
            else
            {

            }

            if (queue.Length > 0)
            {
            }
            else
            {

            }

        }

        private Serverpool ChooseServerpool()
        {
            List<Serverpool> selection = new List<Serverpool>();

            double p = nrServerPools / serverPools.Count;

            for (int i = 0; i < serverPools.Count; i++)
            {
                if (rnd.NextDouble() < p)
                {
                    selection.Add(serverPools[i]);
                    if(selection.Count == nrServerPools) { break; }
                }

                p = (nrServerPools - selection.Count) / (serverPools.Count - i - 1);
            }

            return selection.OrderBy(x => x.JobCount).First();
        }
    }
}
