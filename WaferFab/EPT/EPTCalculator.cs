using CSSL.Examples.WaferFab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaferFabSim.Import;

namespace WaferFabSim.EPT
{
    public class EPTCalculator
    {
        public List<LotActivity> LotActivities { get; private set; }

        /// <summary>
        /// Sorted on departure!
        /// </summary>
        private List<LotActivity> allSortedOnDeparture { get; set; }

        private List<LotActivity> subsetNoDeparture { get; set; }

        private List<LotActivity> subsetNoArrivalDeparture { get; set; }


        public List<LotActivity> CalculateEPTs(List<LotActivity> lotActivities)
        {
            LotActivities = lotActivities.ToList();

            allSortedOnDeparture = LotActivities.OrderBy(x => x.Departure).ToList();

            subsetNoDeparture = LotActivities.Where(x => x.Arrival != null && x.Departure == null).ToList();

            subsetNoArrivalDeparture = LotActivities.Where(x => x.Arrival == null && x.Departure == null).ToList();

            DateTime? prevDepart = null;

            for(int i = 0; i < allSortedOnDeparture.Count; i++)
            {
                LotActivity act = allSortedOnDeparture[i];

                if (act.Departure != null && prevDepart != null && act.WIPIn > -1)
                {
                    TimeSpan ept = new TimeSpan();

                    // Calculate EPT
                    if (act.WIPIn > 0)
                    {
                        ept = (DateTime)act.Departure - (DateTime)prevDepart;
                    }
                    else if (act.WIPIn == 0)
                    {
                        ept = (DateTime)act.Departure - (DateTime)act.Arrival;
                    }                    

                    act.EPT = ept.TotalSeconds;

                    // Calculate overtaken lots (this operation is slow)
                    act.OvertakenLots = subsetNoArrivalDeparture.Count + subsetNoDeparture.Where(x => x.Arrival < act.Arrival).Count() + allSortedOnDeparture.GetRange(i, allSortedOnDeparture.Count - i).Where(x => x.Arrival < act.Arrival).Count();
                }
                prevDepart = act.Departure;
            }

            return LotActivities;
        }
    }
}
