using CSSL.Modeling;
using CSSL.RL;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.AccessController
{
    public class RLLayer : RLLayerBase
    {
        public AccessController ac { get; set; }

        public override void BuildTrainingEnvironment()
        {
            Settings.WriteOutput = false;

            Simulation = new RLSimulation("Access_controller_simulation");

            ac = new AccessController(Simulation.MyModel, "Access_controller", this);
        }
    }
}
