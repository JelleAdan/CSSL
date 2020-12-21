using CSSL.Modeling.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace CSSL.Examples.WaferFab.Dispatchers
{
    public class RandomDispatcher : DispatcherBase
    {
        public RandomDispatcher(ModelElementBase workCenter, string name) : base(workCenter, name)
        {
            rnd = new Random();
        }

        public Random rnd;

        public override void HandleArrival(Lot lot)
        {
            wc.Queues[lot.GetCurrentStep].EnqueueLast(lot);

            // Queue was empty upon arrival, lot gets taken into service and departure event is scheduled immediately
            if (wc.TotalQueueLength == 1)
            {
                ScheduleEvent(GetTime + wc.ServiceTimeDistribution.Next(), wc.HandleDeparture);

                wc.LotStepInService = lot.GetCurrentStep;
            }
        }

        public override void HandleDeparture()
        {
            Lot lot = wc.Queues[wc.LotStepInService].DequeueFirst();

            // Schedule next departure event, if queue is nonempty
            if (wc.TotalQueueLength > 0)
            {
                // Choose random queue to service
                List<LotStep> possibleQueues = wc.Queues.Where(x => x.Value.Length > 0).Select(x => x.Key).ToList();

                wc.LotStepInService = possibleQueues[rnd.Next(0, possibleQueues.Count)];

                ScheduleEvent(GetTime + wc.ServiceTimeDistribution.Next(), wc.HandleDeparture);
            }
            else
            {
                wc.LotStepInService = null;
            }

            // Send to next workcenter. Caution: always put this after the schedule next departure event part.
            // Otherwise it causes problems when a lot has to visit the same workstation twice in a row.
            lot.SendToNextWorkCenter();
        }

        public override void HandleFirstDeparture()
        {
            // Choose biggest queue to service
            wc.LotStepInService = wc.Queues.OrderByDescending(x => x.Value.Length).First().Key;

            HandleDeparture();
        }

        public override void HandleInitialization(List<Lot> lots)
        {
            foreach(Lot lot in lots)
            {
                HandleArrival(lot);
            }
        }
    }
}
