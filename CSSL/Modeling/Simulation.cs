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
            replicationExecutionProcess.TryRun();
        }

        public void End()
        {
            replicationExecutionProcess.TryEnd();
        }
    }

    public class ReplicationExecutionProcess : IterativeProcess<Executive>
    {
        private Experiment experiment;

        public ReplicationExecutionProcess(Experiment experiment)
        {
            this.experiment = experiment;
        }

        protected sealed override void DoInitialize()
        {
            base.DoInitialize();


        }

        protected sealed override void DoRun()
        {
            base.DoRun();
        }

        public void RunNext()
        {
        }
    }
}
