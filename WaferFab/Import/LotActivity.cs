using CSSL.Examples.WaferFab;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using static WaferFabSim.InputDataConversion.AutoDataReader;

namespace WaferFabSim.Import
{
    [Serializable]
    /// <summary>
    /// Lot Activity per workstation. To create an object of a group of LotActivitiesRaw is used
    /// </summary>
    public class LotActivity
    {
        public string LotId => RawActivities.First().LotId;

        public string WorkStation => RawActivities.First().WorkStation;

        public DateTime? Arrival { get; set; } = null;

        public DateTime? Departure { get; set; } = null;

        public TimeSpan? CycleTime => Departure != null && Arrival != null ? Departure - Arrival : null;

        /// <summary>
        /// EPT time in seconds. If -1, this value is n/a or unknown.
        /// </summary>
        public double EPT { get; set; } = -1;

        /// <summary>
        /// WIP right after EPT start. If -1, this value is n/a or unknown.
        /// </summary>
        public int WIPEPTstart { get; set; } = -1;

        /// <summary>
        /// Number of lots that this lot has overtaken. If -1, this value is n/a or unknown.
        /// </summary>
        public int OvertakenLots { get; set; } = -1;

        /// <summary>
        /// WIP right before arrival. If -1, this value is n/a or unknown.
        /// </summary>
        public int WIPIn { get; set; } = -1;

        /// <summary>
        /// WIP right after departure. If -1, this value is n/a or unknown.
        /// </summary>
        public int WIPOut { get; set; } = -1;

        public int QtyIn => RawActivities.Select(x => x.QtyIn).Max();

        public string IRDGroup => RawActivities.First().IRDGroup;

        public string ProductType => RawActivities.First().ProductType;
        
        /// <summary>
        /// Ordered on Track-in time. Do not alter this list without making sure to keep it ordered.
        /// </summary>
        public List<LotActivityRaw> RawActivities { get; private set; }


        public void AddLotActivityRaw(LotActivityRaw raw)
        {
            RawActivities.Add(raw);

            raw.LotActivity = this;
        }

        public LotActivity(LotActivityRaw raw)
        {
            // Connect the two to each other
            RawActivities = new List<LotActivityRaw>();

            RawActivities.Add(raw);
            raw.LotActivity = this;
        }

        // Used for WaferAreaSim (one single area)
        public Lot ConvertToLot(double creationTime, Sequence sequence)
        {
            Lot newLot = new Lot(creationTime, sequence);

            newLot.LotID = LotId;
            newLot.ArrivalReal = Arrival;
            newLot.DepartureReal = Departure;
            newLot.WIPInReal = WIPIn;
            newLot.OvertakenLotsReal = OvertakenLots;
            return newLot;
        }
    }
}
