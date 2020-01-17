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
            MyModel = new Model(name + "_Model", MyExecutive, this);
            MyExperiment = new Experiment(name + "_Experiment");
        }

        public Simulation(string name, Executive executive)
        {
            MyExecutive = executive;
            MyModel = new Model(name + "_Model", MyExecutive, this);
            MyExperiment = new Experiment(name + "_Experiment");
            replicationExecutionProcess = new ReplicationExecutionProcess(this);
        }

        public Executive MyExecutive { get; }

        public Model MyModel { get; }

        public Experiment MyExperiment { get; }

        private ReplicationExecutionProcess replicationExecutionProcess { get; }

        public void TryInitialize()
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
        private Simulation simulation;

        public ReplicationExecutionProcess(Simulation simulation)
        {
            this.simulation = simulation;
        }

        protected override bool HasNext => simulation.MyExperiment.HasMoreReplications;

        protected sealed override void DoInitialize()
        {
            base.DoInitialize();
            simulation.MyExperiment.ResetCurrentReplicationNumber();
            simulation.MyModel.StrictlyDoBeforeExperiment();
        }

        protected sealed override int NextIteration()
        {
            return simulation.MyExperiment.IncrementCurrentReplicationNumber();
        }

        protected sealed override void RunIteration()
        {
            NextIteration();
            simulation.MyExecutive.TryInitialize();
            simulation.MyModel.StrictlyDoBeforeReplication();

        }
    }
}
