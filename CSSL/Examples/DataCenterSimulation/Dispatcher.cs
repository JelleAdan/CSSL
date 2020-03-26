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
    public class Dispatcher : SchedulingElementBase
    {
        public Dispatcher(ModelElementBase parent, string name, Distribution serviceTimeDistribution, double serviceTimeThreshold, List<ServerPool> serverPools, double dispatchTime) : base(parent, name)
        {
            dataCenter = (DataCenter)parent;
            Queue = new CSSLQueue<Job>(this, name + "_Queue");
            this.serviceTimeDistribution = serviceTimeDistribution;
            this.serviceTimeThreshold = serviceTimeThreshold;
            rnd = new Random();
            this.serverPools = serverPools;
            this.dispatchTime = dispatchTime;
            nrServerPoolsToChooseFrom = serverPools.Count();
        }

        public Dispatcher(ModelElementBase parent, string name, Distribution serviceTimeDistribution, double serviceTimeThreshold, List<ServerPool> serverPools, double dispatchTime, int nrServerPools) : base(parent, name)
        {
            dataCenter = (DataCenter)parent;
            Queue = new CSSLQueue<Job>(this, name + "_Queue");
            this.serviceTimeDistribution = serviceTimeDistribution;
            this.serviceTimeThreshold = serviceTimeThreshold;
            rnd = new Random();
            this.serverPools = serverPools;
            this.dispatchTime = dispatchTime;
            this.nrServerPoolsToChooseFrom = nrServerPools;
        }

        public CSSLQueue<Job> Queue;

        public int QueueLength => Queue.Length;

        public int TotalNrJobsInSystem => QueueLength + serverPools.Sum(x => x.JobCount);

        private DataCenter dataCenter { get; set; }

        private Distribution serviceTimeDistribution;

        private double serviceTimeThreshold;

        private Random rnd;

        private List<ServerPool> serverPools;

        private int nrServerPoolsToChooseFrom;

        private double dispatchTime;

        public void HandleArrival(Job job)
        {
            dataCenter.HandleArrival();

            NotifyObservers(this);

            Queue.EnqueueLast(job);

            if (QueueLength == 1) // Queue was empty upon arrival, schedule dispatch event immediately. 
            {
                ScheduleEvent(GetTime + dispatchTime, Dispatch);
            }
        }

        private void Dispatch(CSSLEvent e)
        {
            NotifyObservers(this);

            Job job = Queue.DequeueFirst();

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
            if (QueueLength > 0)
            {
                ScheduleEvent(GetTime + dispatchTime, Dispatch);
            }
        }

        private void SendToServerPool(Job job)
        {
            ServerPool serverPool = ChooseServerpool();

            double departureTime = GetTime + job.ServiceTime;

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

        private ServerPool ChooseServerpool()
        {
            List<ServerPool> selection = new List<ServerPool>();

            double p = (double)nrServerPoolsToChooseFrom / serverPools.Count;

            for (int i = 0; i < serverPools.Count; i++)
            {
                if (rnd.NextDouble() < p)
                {
                    selection.Add(serverPools[i]);
                    if(selection.Count == nrServerPoolsToChooseFrom) { break; }
                }

                p = ((double)nrServerPoolsToChooseFrom - selection.Count) / (serverPools.Count - i - 1);
            }

            return selection.OrderBy(x => x.JobCount).First();
        }
    }
}
