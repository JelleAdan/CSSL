using CSSL.Modeling;
using System;
using System.Collections.Generic;
using System.Text;

namespace WaferFabSim
{
    public class ExperimentSettings
    {
        public int NumberOfReplications { get; set; }
        public double LengthOfWarmUp { get; set; }
        public double LengthOfReplication { get; set; }
        public double LengthOfReplicationWallClock { get; set; }

        public ExperimentSettings()
        {
            LengthOfWarmUp = double.MaxValue;
            LengthOfReplication = double.MaxValue;
            LengthOfReplicationWallClock = double.MaxValue;
        }
    }
}
