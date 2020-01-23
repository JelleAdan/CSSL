using CSSL.Calendar;
using CSSL.Modeling.Elements;
using CSSL.Observer;
using CSSL.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling
{
    public class Simulation : IDisposable
    {
        public Simulation(string name, string outputDirectory)
        {
            MyExecutive = new Executive(this);
            MyModel = new Model(name + "_Model", this);
            MyExperiment = new Experiment(name + "_Experiment", outputDirectory);
            MyObservers = new List<IDisposable>();
            replicationExecutionProcess = new ReplicationExecutionProcess(this);
            OutputDirectory = outputDirectory;
        }

        public Simulation(string name, string outputDirectory, Executive executive)
        {
            MyExecutive = executive;
            MyModel = new Model(name + "_Model", this);
            MyExperiment = new Experiment(name + "_Experiment", outputDirectory);
            MyObservers = new List<IDisposable>();
            replicationExecutionProcess = new ReplicationExecutionProcess(this);
            OutputDirectory = outputDirectory;
        }

        public Executive MyExecutive { get; }

        public Model MyModel { get; }

        public Experiment MyExperiment { get; }

        internal readonly List<IDisposable> MyObservers;

        private ReplicationExecutionProcess replicationExecutionProcess { get; }

        internal string OutputDirectory { get; }

        public void TryInitialize()
        {
            replicationExecutionProcess.TryInitialize();
        }

        public void TryRun()
        {
            try
            {
                replicationExecutionProcess.TryRunAll();
            }
            finally
            {
                
            }
        }

        public void TryRunNext()
        {
            replicationExecutionProcess.TryRunNext();
        }

        public void End()
        {
            replicationExecutionProcess.TryEnd();
        }

        public void Dispose()
        {
            
        }

        private void DisposeObservers()
        {
            foreach(IDisposable observer in MyObservers)
            {
                observer.Dispose();
            }
        }
    }

    public class ReplicationExecutionProcess : IterativeProcess<int>
    {
        private Simulation simulation;

        public ReplicationExecutionProcess(Simulation simulation)
        {
            this.simulation = simulation;
        }

        protected override double maxComputationalTimeMiliseconds => simulation.MyExperiment.MaxComputationalTimeTotal;

        protected override bool HasNext => simulation.MyExperiment.HasMoreReplications;

        protected sealed override void DoInitialize()
        {
            base.DoInitialize();
            simulation.MyExperiment.ResetCurrentReplicationNumber();
            simulation.MyExperiment.CreateExperimentOutputDirectory();
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
            simulation.MyExperiment.CreateReplicationOutputDirectory();
            simulation.MyModel.StrictlyDoBeforeReplication();
            simulation.MyExecutive.TryRunAll();
        }
    }
}
