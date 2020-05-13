using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSSL.Modeling.Elements;

namespace CSSL.Examples.WaferFab.Dispatchers
{
    // Biggest Queue First Dispatcher
    public class BQFDispatcher : DispatcherBase
    {
        public BQFDispatcher(ModelElementBase workCenter, string name) : base(workCenter, name)
        {
        }

        public override void HandleArrival(Lot lot)
        {
            Console.WriteLine($"{GetTime} Arrival \tlot {lot.Id} \t{lot.GetCurrentStep.Name} \t{wc.Name}");

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

            //Console.WriteLine($"{GetTime} Departure \tlot {lot.Id} \t{lot.GetCurrentStep.Name} \t{wc.Name}");

            // Schedule next departure event, if queue is nonempty
            if (wc.TotalQueueLength > 0)
            {
                // Choose biggest queue to service
                wc.LotStepInService = wc.Queues.OrderByDescending(x => x.Value.Length).First().Key;

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
    }
}
