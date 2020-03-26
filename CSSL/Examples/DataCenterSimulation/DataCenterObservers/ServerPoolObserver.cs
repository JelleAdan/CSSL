using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Observer;
using CSSL.Utilities.Statistics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.DataCenterSimulation.DataCenterObservers
{
    public class ServerPoolObserver : ModelElementObserverBase
    {
        private Variable<int> nrOfJobs;
        private WeightedStatistic nrOfJobsStatistic;


        public ServerPoolObserver(Simulation sim) : base(sim)
        {
            nrOfJobs = new Variable<int>(this);
            nrOfJobsStatistic = new WeightedStatistic("Number of jobs per serverpool");
        }

        protected override void OnExperimentStart(ModelElementBase modelElement)
        {
        }

        protected override void OnReplicationStart(ModelElementBase modelElement)
        {
            nrOfJobs.Reset();
            nrOfJobsStatistic.Reset();
        }

        protected override void OnWarmUp(ModelElementBase modelElement)
        {
        }

        protected override void OnInitialized(ModelElementBase modelElement)
        {
            ServerPool serverPool = (ServerPool)modelElement;
            nrOfJobs.UpdateValue(serverPool.JobCount);

            Writer.WriteLine($"Current Simulation Time,Average number of jobs, StDev on average number of jobs");
        }

        protected override void OnUpdate(ModelElementBase modelElement)
        {
            ServerPool serverPool = (ServerPool)modelElement;

            nrOfJobs.UpdateValue(serverPool.JobCount);
            nrOfJobsStatistic.Collect(nrOfJobs.Value, nrOfJobs.Weight);

            Writer.WriteLine($"{serverPool.GetTime}',{nrOfJobsStatistic.Average()},{nrOfJobsStatistic.StandardDeviation()}");
        }

        protected override void OnReplicationEnd(ModelElementBase modelElement)
        {
        }

        protected override void OnExperimentEnd(ModelElementBase modelElement)
        {
        }

        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }
    }
}
