using CSSL.Examples.WaferFab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaferFabSim.SnapshotData
{
    public class RealSnapshot : WIPSnapshotBase
    {
        public DateTime Time { get; private set; }

        public RealSnapshot(List<RealLot> lots, int waferQtyThreshold)
        {
            LotSteps = lots.Select(x => x.IRDGroup).Distinct().ToArray();

            Time = lots.First().SnapshotTime;

            foreach(var lotStep in LotSteps)
            {
                WIPlevels.Add(lotStep, lots.Where(x => x.IRDGroup == lotStep).Where(x => x.Qty >= waferQtyThreshold).Count());
            }
        }
    }
}
