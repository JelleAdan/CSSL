using CSSL.Modeling;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.RL
{
    public class RLSimulation : Simulation
    {
        public RLSimulation(string name, string outputDirectory = null) : base(name, outputDirectory)
        {
            MyExperiment.LengthOfReplication = double.PositiveInfinity;
        }

        public void StartTrain()
        {
            MyExperiment.StrictlyOnExperimentStart(true);

            MyModel.StrictlyOnExperimentStart();

            MyExecutive.TryInitialize();

            MyExperiment.StrictlyOnReplicationStart();

            MyModel.StrictlyOnReplicationStart();
        }

        public void Train()
        {
            MyExecutive.TryRunAll();
        }

        public void EndTrain()
        {
            MyModel.StrictlyOnReplicatioEnd();

            MyModel.StrictlyOnExperimentEnd();
        }
    }
}
