using CSSL.Modeling;
using CSSL.Modeling.CSSLQueue;
using CSSL.Modeling.Elements;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Examples.DataCenterSimulation
{
    public class Dispatcher : SchedulingElement
    {
        public Dispatcher(ModelElement parent, string name, Distribution serviceTimeDistribution, double serviceTimeThreshold, List<Serverpool> serverPools, double dispatchTime) : base(parent, name)
        {
            queue = new CSSLQueue<Job>(this, name + "_Queue");
            this.serviceTimeDistribution = serviceTimeDistribution;
            this.serviceTimeThreshold = serviceTimeThreshold;
            rnd = new Random();
            this.serverPools = serverPools;
            this.dispatchTime = dispatchTime;
        }

        private CSSLQueue<Job> queue;

        public int QueueLength => queue.Length;

        private Distribution serviceTimeDistribution;

        private double serviceTimeThreshold;

        private Random rnd;

        private List<Serverpool> serverPools;

        private int nrServerPools => serverPools.Count;

        private double dispatchTime;

        public void HandleArrival(Job job)
        {
            NotifyObservers(this);

            queue.EnqueueLast(job);

            if (queue.Length == 1)
            {
                ScheduleEvent(GetTime, Dispatch);
            }
        }

        private void Dispatch(CSSLEvent e)
        {
            Job job = queue.DequeueFirst();

            job.ServiceTime = serviceTimeDistribution.Next();

            if (job.ServiceTime > serviceTimeThreshold)
            {
                Job job2 = SplitJobs(job);
                SendToServerPool(job);
                SendToServerPool(job2);
            }
            else
            {
                SendToServerPool(job);
            }

            // Schedule next dispatch event, if queue is nonempty
            if (queue.Length > 0)
            {
                ScheduleEvent(GetTime + dispatchTime, Dispatch);
            }
        }

        private void SendToServerPool(Job job)
        {
            Serverpool serverPool = ChooseServerpool();

            double departureTime = GetTime + job.ServiceTime;

            ScheduleEvent(departureTime, serverPool.HandleDeparture);

            job.DepartureTime = departureTime;

            serverPool.HandleArrival(job);
        }

        private Job SplitJobs(Job job)
        {
            double serviceTime1 = job.ServiceTime * rnd.NextDouble();
            double serviceTime2 = job.ServiceTime - serviceTime1;

            Job job2 = new Job(job.CreationTime);

            job.ServiceTime = serviceTime1;
            job2.ServiceTime = serviceTime2;

            job.DepartureTime = GetTime + serviceTime1;
            job2.DepartureTime = GetTime + serviceTime2;

            return job2;
        }

        private Serverpool ChooseServerpool()
        {
            Console.WriteLine("Choose serverpool.");

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
