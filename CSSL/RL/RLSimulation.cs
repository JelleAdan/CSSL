using CSSL.Modeling;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.RL
{
    public class RLSimulation : Simulation
    {
        public RLSimulation(string name) : base(name)
        {
            MyExperiment.LengthOfReplication = double.PositiveInfinity;
        }

        public void StartTrain()
        {
            MyModel.StrictlyOnExperimentStart();

            MyExecutive.TryInitialize();

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
