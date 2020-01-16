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
            replicationExecutionProcess = new ReplicationExecutionProcess(MyExperiment);
        }

        public Executive MyExecutive { get; }

        public Model MyModel { get; private set; }

        public Experiment MyExperiment { get; }

        private ReplicationExecutionProcess replicationExecutionProcess { get; }

        public void Initialize()
        {
            replicationExecutionProcess.TryInitialize();
        }

        public void Run()
        {
            replicationExecutionProcess.TryRunAll();
        }

        public void RunNext()
        {
            replicationExecutionProcess.TryRunNext();
        }

        public void End()
        {
            replicationExecutionProcess.TryEnd();
        }
    }

    public class ReplicationExecutionProcess : IterativeProcess<int>
    {
        private Experiment experiment;

        public ReplicationExecutionProcess(Experiment experiment)
        {
            this.experiment = experiment;
        }

        protected override bool HasNext => experiment.HasMoreReplications;

        protected sealed override void DoInitialize()
        {
            base.DoInitialize();
            experiment.ResetCurrentReplicationNumber();
        }

        protected sealed override int NextIteration()
        {
            return experiment.IncrementCurrentReplicationNumber();
        }

        protected sealed override void RunIteration()
        {
            

        }
    }
}
