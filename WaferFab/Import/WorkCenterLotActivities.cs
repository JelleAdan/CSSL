using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Transactions;
using WaferFabSim.EPT;
using WaferFabSim.Import;

namespace WaferFabSim.InputDataConversion
{
    [Serializable]

    public class WorkCenterLotActivities
    {

        public string WorkCenter { get; set; }

        public List<LotActivity> LotActivities { get; set; }

        private List<Tuple<DateTime, int, LotActivity>> wipCalculations { get; set; }

        private DateTime startDate { get; set; }

        private DateTime endDate { get; set; }

        /// <summary>
        /// WIP qty immediately right after a timestamp. This list is ordered on time.
        /// </summary>
        public List<Tuple<DateTime, int>> WIPTrace { get; set; } 

        private List<DateTime> wipTraceTimes { get; set; } 


        public WorkCenterLotActivities(LotTraces lotTraces, string workCenter)
        {
            WorkCenter = workCenter;

            startDate = lotTraces.StartDate;
            endDate = lotTraces.EndDate;

            WIPTrace = new List<Tuple<DateTime, int>>();

            LotActivities = lotTraces.LotActivities.Where(x => x.WorkStation == workCenter).OrderBy(x => x.Arrival).ToList();

            CalculateWIPTrace(startDate, endDate);

            EPTCalculator EPTCalculator = new EPTCalculator();

            LotActivities = EPTCalculator.CalculateEPTs(LotActivities);

            AddWIPsToEPTstarts();
        }

        /// <summary>
        /// Calculate WIP trace and assign WIPin and WIPout qtys to LotActvities
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public void CalculateWIPTrace(DateTime startDate, DateTime endDate)
        {
            // Determine begin qty
            int beginWIP = LotActivities.Where(x => (x.Arrival == null || x.Arrival < startDate) && (x.Departure == null || x.Departure > startDate)).Count();

            WIPTrace.Add(new Tuple<DateTime, int>(startDate, beginWIP));

            wipCalculations = new List<Tuple<DateTime, int, LotActivity>>();

            // Add arrivals to WIPcalculations
            foreach(LotActivity activity in LotActivities.Where(x => x.Arrival != null && x.Arrival >= startDate && x.Arrival <= endDate))
            {
                wipCalculations.Add(new Tuple<DateTime, int, LotActivity>((DateTime)activity.Arrival, +1, activity));
            }

            // Add departures to WIPcalculations
            foreach (LotActivity activity in LotActivities.Where(x => x.Departure != null && x.Departure >= startDate && x.Departure <= endDate))
            {
                wipCalculations.Add(new Tuple<DateTime, int, LotActivity>((DateTime)activity.Departure, -1, activity));
            }

            int currentWIP = beginWIP;

            // Order WIP calculations on time
            wipCalculations = wipCalculations.OrderBy(x => x.Item1).ToList();

            // Loop through arrival and departure events and generate WIP Trace
            foreach (Tuple<DateTime, int, LotActivity> calc in wipCalculations)
            {
                if (calc.Item1 == WIPTrace.Last().Item1)
                {
                    WIPTrace.RemoveAt(WIPTrace.Count() -1);
                }

                currentWIP += calc.Item2;

                WIPTrace.Add(new Tuple<DateTime, int>(calc.Item1, currentWIP));

                // Assign WIPin qty (right before arrival) to LotActivity
                if (calc.Item2 == 1)
                {
                    calc.Item3.WIPIn = currentWIP - 1;
                }
                // Assign WIPout qty (right after departure) to LotActivity
                else if (calc.Item2 == -1)
                {
                    calc.Item3.WIPOut = currentWIP;
                }
            }

            // Check if last WIP value is correct
            int endWIP = LotActivities.Where(x => (x.Departure == null || x.Departure > endDate) && (x.Arrival == null || x.Arrival < endDate)).Count();

            if (endWIP != WIPTrace.Last().Item2)
            {
                throw new Exception($"End WIP does not match Begin WIP + sum(WIP changes) for {WorkCenter}");
            }

            wipTraceTimes = WIPTrace.Select(x => x.Item1).ToList();
        }

        /// <summary>
        /// Binary search through WIP trace to get WIP right after time
        /// </summary>
        /// <param name="time">Time of WIP</param>
        /// <returns></returns>
        public int GetWIPAfterTime(DateTime time)
        {
            if (time >= startDate && time <= endDate)
            {
                int index = wipTraceTimes.BinarySearch(time);

                return WIPTrace[index].Item2;
            }
            else
            {
                throw new Exception($"WIP after time {time} is unknown. WIP is only know between {startDate} and {endDate}");
            }
        }

        public int GetWIPBeforeTime(DateTime time)
        {
            if (time > startDate && time <= endDate)
            {
                int index = WIPTrace.Select(x => x.Item1).ToList().BinarySearch(time);

                return WIPTrace[index - 1].Item2;
            }
            else if (time == startDate)
            {
                return -1;
            }
            else
            {
                throw new Exception($"WIP before time {time} is unknown. WIP is only know between {startDate} and {endDate}");
            }
        }

        public void AddWIPsToEPTstarts()
        {
            foreach (LotActivity activity in LotActivities)
            {
                if (activity.EPT >= 0)
                {
                    DateTime departure = (DateTime)activity.Departure;

                    activity.WIPEPTstart = GetWIPAfterTime(departure.AddSeconds(-activity.EPT));
                }
            }
        }
    }
}
