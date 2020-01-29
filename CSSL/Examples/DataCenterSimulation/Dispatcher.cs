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
        public Dispatcher(ModelElementBase parent, string name, Distribution serviceTimeDistribution, double serviceTimeThreshold, List<ServerPool> serverPools, double dispatchTime) : base(parent, name)
        {
            queue = new CSSLQueue<Job>(this, name + "_Queue");
            this.serviceTimeDistribution = serviceTimeDistribution;
            this.serviceTimeThreshold = serviceTimeThreshold;
            rnd = new Random();
            this.serverPools = serverPools;
            this.dispatchTime = dispatchTime;
            nrServerPools = serverPools.Count();
        }

        public Dispatcher(ModelElementBase parent, string name, Distribution serviceTimeDistribution, double serviceTimeThreshold, List<ServerPool> serverPools, double dispatchTime, int nrServerPools) : base(parent, name)
        {
            queue = new CSSLQueue<Job>(this, name + "_Queue");
            this.serviceTimeDistribution = serviceTimeDistribution;
            this.serviceTimeThreshold = serviceTimeThreshold;
            rnd = new Random();
            this.serverPools = serverPools;
            this.dispatchTime = dispatchTime;
            this.nrServerPools = nrServerPools;
        }

        private CSSLQueue<Job> queue;

        public int QueueLength => queue.Length;

        public int TotalNrJobsInSystem => QueueLength + serverPools.Sum(x => x.JobCount);

        private Distribution serviceTimeDistribution;

        private double serviceTimeThreshold;

        private Random rnd;

        private List<ServerPool> serverPools;

        private int nrServerPools;

        private double dispatchTime;

        public void HandleArrival(Job job)
        {
            NotifyObservers(this);

            queue.EnqueueLast(job);

            if (queue.Length == 1) // Queue was empty upon arrival, schedule dispatch event immediately. 
            {
                ScheduleEvent(GetSimulationTime + dispatchTime, Dispatch);
            }
        }

        private void Dispatch(CSSLEvent e)
        {
            NotifyObservers(this);

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
                ScheduleEvent(GetSimulationTime + dispatchTime, Dispatch);
            }
        }

        private void SendToServerPool(Job job)
        {
            ServerPool serverPool = ChooseServerpool();

            double departureTime = GetSimulationTime + job.ServiceTime;

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

            job.DepartureTime = GetSimulationTime + serviceTime1;
            job2.DepartureTime = GetSimulationTime + serviceTime2;

            return job2;
        }

        private ServerPool ChooseServerpool()
        {
            List<ServerPool> selection = new List<ServerPool>();

            double p = (double)nrServerPools / serverPools.Count;

            for (int i = 0; i < serverPools.Count; i++)
            {
                if (rnd.NextDouble() < p)
                {
                    selection.Add(serverPools[i]);
                    if(selection.Count == nrServerPools) { break; }
                }

                p = ((double)nrServerPools - selection.Count) / (serverPools.Count - i - 1);
            }

            return selection.OrderBy(x => x.JobCount).First();
        }
    }
}
