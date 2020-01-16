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
        public Dispatcher(ModelElement parent, string name, Distribution serviceTimeDistribution, double serviceTimeThreshold, List<Serverpool> serverPools, int nrServerPools, double epsilon) : base(parent, name)
        {
            queue = new CSSLQueue<CSSLQueueObject>(parent, name + "_Queue");
            this.serviceTimeDistribution = serviceTimeDistribution;
            this.serviceTimeThreshold = serviceTimeThreshold;
            rnd = new Random();
            this.serverPools = serverPools;
            this.nrServerPools = nrServerPools;
            this.epsilon = epsilon;
        }

        private CSSLQueue<CSSLQueueObject> queue;

        private Distribution serviceTimeDistribution;

        private double serviceTimeThreshold;

        private Random rnd;

        private List<Serverpool> serverPools;

        private int nrServerPools;

        private double epsilon;

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
                DispatchTwoJobs(job, serviceTime);
            }
            else
            {
                DispatchOneJob(job, serviceTime);
            }

            // Schedule next dispatch event, if queue is nonempty
            if (queue.Length > 0)
            {
                ScheduleEvent(GetTime() + epsilon, Dispatch);
            }
            else
            {

            }

        }

        private void DispatchOneJob(Job job, double serviceTime)
        {
            Serverpool serverPool = ChooseServerpool();

            double departureTime = GetTime() + serviceTime;

            ScheduleEvent(departureTime, serverPool.HandleDeparture);

            job.DepartureTime = departureTime;

            serverPool.HandleArrival(job);
        }

        private void DispatchTwoJobs(Job job, double serviceTime)
        {
            double serviceTime1 = serviceTime * rnd.NextDouble();
            double serviceTime2 = serviceTime - serviceTime1;

            Serverpool serverPool1 = ChooseServerpool();
            Serverpool serverPool2 = ChooseServerpool();

            double departureTime1 = GetTime() + serviceTime1;
            double departureTime2 = GetTime() + serviceTime2;

            Job job2 = new Job(GetTime());

            job.DepartureTime = departureTime1;
            job2.DepartureTime = departureTime2;

            ScheduleEvent(GetTime() + serviceTime1, serverPool1.HandleDeparture);
            ScheduleEvent(GetTime() + serviceTime2, serverPool2.HandleDeparture);

            serverPool1.HandleArrival(job);
            serverPool2.HandleArrival(job);
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

        public void SendToServerpool(Serverpool serverPool)
        {

        }
    }
}
