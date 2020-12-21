using CSSL.Examples.WaferFab.Utilities;
using CSSL.Modeling.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace CSSL.Examples.WaferFab.Dispatchers
{
    public class EPTOvertakingDispatcher : DispatcherBase
    {

        public EPTOvertakingDispatcher(ModelElementBase workCenter, string name, OvertakingDistributionBase overtakingDistribution) : base(workCenter, name)
        {
            OvertakingDistribution = overtakingDistribution;
        }

        public OvertakingDistributionBase OvertakingDistribution { get; }

        public override void HandleArrival(Lot lot)
        {
            int overtaking = (int)OvertakingDistribution.Next();

            // Temporary for WaferAreaSim
            lot.WIPIn = wc.TotalQueueLength;

            //Console.WriteLine($"{Parent.GetTime} lot {lot.LotID} enters workstation {wc.Name} and overtakes {overtaking} out of {wc.Queue.Length}");

            // Use overtaking distribution
            wc.Queues[lot.GetCurrentStep].EnqueueLast(lot);
            wc.Queue.EnqueueAt(lot, wc.Queue.Length - overtaking);

            // Queue was empty upon arrival, lot gets taken into service and departure event is scheduled immediately
            if (wc.Queue.Length == 1)
            {
                ScheduleEvent(GetTime + wc.ServiceTimeDistribution.Next(), wc.HandleDeparture);

                wc.LotStepInService = lot.GetCurrentStep;
            }
            // Lot overtook complete queue and is now first
            else if (overtaking == wc.Queue.Length - 1)
            {
                wc.LotStepInService = lot.GetCurrentStep;
            }
        }

        public override void HandleDeparture()
        {
            // Simple FCFS from queue
            DepartingLot = wc.Queue.DequeueFirst();

            //Console.WriteLine($"{Parent.GetTime} lot {lot.LotID} leaves workstation {wc.Name}. LotStepInService {wc.LotStepInService.Name} and Lotstep is {lot.GetCurrentStep.Name}");

            int sizebefore = wc.Queues[wc.LotStepInService].Length;

            // Remove same lot from seperate queue
            wc.Queues[wc.LotStepInService].Dequeue(DepartingLot);

            int sizeafter = wc.Queues[wc.LotStepInService].Length;

            //// For performance reasons this can be changed to:
            //wc.Queues[wc.LotStepInService].DequeueFirst();

            // Schedule next departure event, if queue is nonempty
            if (wc.TotalQueueLength > 0)
            {
                wc.LotStepInService = wc.Queue.PeekFirst().GetCurrentStep;

                ScheduleEvent(GetTime + wc.ServiceTimeDistribution.Next(), wc.HandleDeparture);
            }
            else
            {
                wc.LotStepInService = null;
            }

            // Send to next workcenter. Caution: always put this after the schedule next departure event part.
            // Otherwise it causes problems when a lot has to visit the same workstation twice in a row.
            DepartingLot.SendToNextWorkCenter();

            // Lot ended production, report lot out to observer.
            if (!DepartingLot.HasNextStep)
            {
                NotifyObservers(this);
            }
        }

        public override void HandleFirstDeparture()
        {
            wc.LotStepInService = wc.Queue.PeekFirst().GetCurrentStep;

            HandleDeparture();
        }

        public override void HandleInitialization(List<Lot> lots)
        {
            Console.WriteLine("lotswitharrival for workcenter " + wc.Name);

            if (lots.Any())
            {
                List<Lot> lotsWithArrival = lots.Where(x => x.ArrivalReal != null).OrderBy(x => x.ArrivalReal).ToList();
                List<Lot> lotsWithoutArrival = lots.Where(x => x.ArrivalReal == null).ToList();

                foreach (Lot lot in lotsWithArrival)
                {
                    Console.WriteLine(lot.LotID);
                }

                List<Lot> queue = new List<Lot>();

                // First add lots with known arrival and WIP in
                if (lotsWithArrival.Any())
                {
                    int WIPBegin = lotsWithArrival.First().WIPInReal;

                    // Initialize queue with WIPbegin number of null lots
                    for (int i = 0; i < WIPBegin; i++)
                    {
                        queue.Add(null);
                    }

                    // Remove null lots at begin or add null lots at end of lot to match lot's WIPbegin.
                    foreach (Lot lot in lotsWithArrival)
                    {
                        int difference = lot.WIPInReal - queue.Count;

                        if (difference > 0)
                        {
                            queue = addNullLots(queue, difference);
                        }
                        else if (difference < 0)
                        {
                            queue = removeNullLots(queue, -1 * difference);
                        }

                        wc.LastArrivedLot = lot;

                        int overtaking = (int)OvertakingDistribution.Next(lot, queue.Count);

                        queue.Insert(queue.Count - overtaking, lot);
                    }
                }

                // Randomly add lots with unknown arrival and WIP in
                if (lotsWithoutArrival.Any())
                {
                    foreach (Lot lot in lotsWithoutArrival)
                    {
                        int overtaking = (int)OvertakingDistribution.Next(lot, queue.Count);

                        queue.Insert(queue.Count - overtaking, lot);
                    }
                }

                List<Lot> finalqueue = queue.Where(x => x != null).ToList();

                Console.WriteLine($"Intialization for {wc.Name}");

                // Finally initialize queue
                foreach (Lot lot in queue.Where(x => x != null))
                {
                    wc.Queues[lot.GetCurrentStep].EnqueueLast(lot);
                    wc.Queue.EnqueueLast(lot);

                    Console.WriteLine(lot.LotID);
                }

                // Departure for first lot in the queue is scheduled.
                ScheduleEvent(GetTime + wc.ServiceTimeDistribution.Next(), wc.HandleDeparture);

                wc.LotStepInService = wc.Queue.PeekFirst().GetCurrentStep;
            }
        }

        private List<Lot> addNullLots(List<Lot> list, int count)
        {
            for (int i = 0; i < count; i++)
            {
                list.Add(null);
            }

            return list;
        }

        /// <summary>
        /// Removes a specified number of null lots from beginning of list. If less null lots than specified number to remove are present,
        /// then all null lots are removed
        /// </summary>
        /// <param name="list"></param>
        /// <param name="numberToRemove"></param>
        /// <returns></returns>
        private List<Lot> removeNullLots(List<Lot> list, int numberToRemove)
        {
            for (int i = 0; i < numberToRemove; i++)
            {
                int index = 0;

                // Find first null lot and remove
                while (index < list.Count())
                {
                    if (list[index] == null)
                    {
                        list.RemoveAt(index);
                        break;
                    }
                    index++;
                }
            }

            return list;
        }
    }
}
