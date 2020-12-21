using CSSL.Examples.WaferFab;
using CSSL.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaferFabSim.SnapshotData
{
    [Serializable]
    public abstract class WIPSnapshotBase
    {
        public string[] LotSteps { get; protected set; }

        public Dictionary<string, int> WIPlevels { get; protected set; }

        public int TotalWIP => WIPlevels.Values.Sum();

        public WIPSnapshotBase()
        {
            WIPlevels = new Dictionary<string, int>();
        }

    }
}
