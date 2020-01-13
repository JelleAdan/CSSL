using CSSL.Calendar;
using CSSL.Modeling.Elements;
using CSSL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling
{
    public class Simulation
    {
        public Simulation(string name)
        {
            MyExecutive = new Executive();
            MyModel = new Model(name + "_Model", MyExecutive);
            MyExperiment = new Experiment(name + "_Experiment");
        }

        public Simulation(string name, Executive executive)
        {
            MyExecutive = executive;
            MyModel = new Model(name + "_Model", MyExecutive);
            MyExperiment = new Experiment(name + "_Experiment");
        }

        public Executive MyExecutive { get; }

        public Experiment MyExperiment { get; }

        public Model MyModel { get; private set; }

        private ReplicationExecutionProcess replicationExecutionProcess { get; }

        public void Initialize()
        {
            replicationExecutionProcess.TryInitialize();
        }
    }

    public class ReplicationExecutionProcess : IterativeProcess<Experiment>
    {
        private List<Experiment> Experiments;

        public ReplicationExecutionProcess(List<Experiment> experiments)
        {
            Experiments = experiments;
        }

        public void RunNext()
        {
            int counter = 0;
            var next = Experiments[counter];
        }
    }
}
