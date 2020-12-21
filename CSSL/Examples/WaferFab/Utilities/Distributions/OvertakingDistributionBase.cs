using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSSL.Examples.WaferFab.Utilities
{
    [Serializable]
    /// <summary>
    /// Base class for WIP dependent discrete overtaking distributions used in the EPTOvertakingDispatcher.cs 
    /// </summary>
    public abstract class OvertakingDistributionBase : Distribution
    {
        public OvertakingDistributionBase(List<OvertakingRecord> records) : base(records.Select(x => x.OvertakenLots).Average(), records.Select(x => x.OvertakenLots).Average())
        {

        }

        /// <summary>
        /// Used for initialization.
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="WIP"></param>
        /// <returns></returns>
        public abstract double Next(Lot lot, int WIP);

        public abstract WorkCenter WorkCenter { get; set; }

        public class OvertakingRecord
        {
            public string WorkCenter { get; set; }
            public string LotStep { get; set; }
            public int WIPIn { get; set; }
            public int OvertakenLots { get; set; }
        }

        public class OvertakingDistributionParameters
        {
            public int MinRecordsPerInterval { get; set; }
            public int MaxNrIntervals { get; set; }
            public int MinSizeInterval { get; set; }

            public OvertakingDistributionParameters(int minPerInterval, int maxNrIntervals, int minSizeInterval)
            {
                MinRecordsPerInterval = minPerInterval;
                MaxNrIntervals = maxNrIntervals;
                MinSizeInterval = minSizeInterval;
            }
        }
    }
}
