using CSSL.Modeling;
using CSSL.Modeling.CSSLQueue;
using CSSL.Modeling.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Examples.DataCenterSimulation
{
    public class ServerPool : SchedulingElementBase
    {
        public ServerPool(ModelElementBase parent, string name) : base(parent, name)
        {
            dataCenter = (DataCenter)parent;
            queue = new ServerPoolQueue(this, name + "_Queue");
        }

        public static int NrJobsDispatched;

        public int GetNrJobsDispatched => NrJobsDispatched;
        
        private DataCenter dataCenter { get; set; }

        private ServerPoolQueue queue { get; set; }

        public int JobCount => queue.Length;

        private void HandleDeparture(CSSLEvent e)
        {
            dataCenter.HandleDeparture();

            NotifyObservers(this);

            Job job = queue.DequeueFirst();

            NrJobsDispatched++;

            if (job.DepartureTime != GetTime)
            {
                throw new Exception($"Departure time of job {job.Id} is {job.DepartureTime} and does not match current time {GetTime}");
            }
        }

        public void HandleArrival(Job job)
        {
            NotifyObservers(this);

            ScheduleEvent(job.DepartureTime, HandleDeparture);

            queue.EnqueueAndSort(job);
        }

        private class ServerPoolQueue : CSSLQueue<Job>
        {
            public ServerPoolQueue(ModelElementBase parent, string name) : base(parent, name)
            {
            }

            /// <summary>
            /// Adds the job to the queue and sorts the queue on departure times.
            /// </summary>
            /// <param name="job">The job to be added.</param>
            public void EnqueueAndSort(Job job)
            {
                items.Add(job);

                items = items.OrderBy(x => x.DepartureTime).ToList();
            }
        }
    }
}
