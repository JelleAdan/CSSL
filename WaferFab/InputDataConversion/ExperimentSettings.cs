using CSSL.Modeling;
using System;
using System.Collections.Generic;
using System.Text;

namespace WaferFabSim.InputDataConversion
{
    public class ExperimentSettings
    {
        public string OutputDirectory { get; set; }
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

        public void UpdateSettingsInSimulation(Simulation sim)
        {
            sim.MyExperiment.NumberOfReplications = NumberOfReplications;
            sim.MyExperiment.LengthOfWarmUp = LengthOfWarmUp;
            sim.MyExperiment.LengthOfReplication = LengthOfReplication;
            sim.MyExperiment.LengthOfReplicationWallClock = LengthOfReplicationWallClock;
        }
    }
}
