using CSSL.Calendar;
using CSSL.Modeling.Elements;
using CSSL.Observer;
using CSSL.Reporting;
using CSSL.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling
{
    public class Simulation : IDisposable, IName, IGetTime
    {
        public Simulation(string name, string outputDirectory)
        {
            Name = name;
            MyExecutive = new Executive(this);
            MyModel = new Model(name + "_Model", this);
            MyExperiment = new Experiment(name + "_Experiment", outputDirectory);
            MyObservers = new Observers();
            replicationExecutionProcess = new ReplicationExecutionProcess(this);
            OutputDirectory = outputDirectory;
        }

        public Executive MyExecutive { get; }

        public Model MyModel { get; }

        public Experiment MyExperiment { get; }

        internal Observers MyObservers { get; }

        private ReplicationExecutionProcess replicationExecutionProcess { get; }

        internal string OutputDirectory { get; }

        public string Name { get; }

        public string GetEndStateIndicator => replicationExecutionProcess.MyEndStateIndicator.ToString();

        public double GetTime => MyExecutive.Time;

        public double GetPreviousEventTime => MyExecutive.PreviousEventTime;

        public double GetWallClockTime => MyExecutive.WallClockTime;

        public TimeSpan GetWallClockTimeSpan => replicationExecutionProcess.GetWallClockTimeSpan;


        public void Run()
        {
            try
            {
                replicationExecutionProcess.TryRunAll();
            }
            finally
            {
                Dispose();
            }
        }

        public void End()
        {
            replicationExecutionProcess.TryEnd();
        }

        public void Dispose()
        {
            MyObservers.Dispose();
        }

        public SimulationReporter MakeSimulationReporter()
        {
            return new SimulationReporter(this);
        }
    }

    public class ReplicationExecutionProcess : IterativeProcess<int>
    {
        private Simulation simulation;

        public ReplicationExecutionProcess(Simulation simulation)
        {
            this.simulation = simulation;
        }

        protected override double maxWallClockTime => simulation.MyExperiment.LengthOfExperimentWallClock * 1000;

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
            simulation.MyObservers.StrictlyDoBeforeReplication();
            simulation.MyModel.StrictlyDoBeforeReplication();
            simulation.MyExecutive.TryRunAll();
            simulation.MyModel.StrictlyDoAfterReplication();
            simulation.MyObservers.StrictlyDoAfterReplication();
        }
    }
}
