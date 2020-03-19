using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Observer;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.DataCenterSimulation.DataCenterObservers
{
    public class ServerPoolObserver : ModelElementObserverBase
    {
        private double averageNrOfJobs;
        private double averageNrOfJobs2;
        private double sigmaNrOfJobs;
        private double oldTime;
        private double currentTime;
        private double sumNrOfJobs;
        private double sumNrOfJobs2;
        
        private double sumTime;

        public ServerPoolObserver(Simulation sim) : base(sim)
        {
        }

        public override void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        protected override void OnInitialized(ModelElementBase modelElement)
        {
            oldTime = modelElement.GetTime;
            sumNrOfJobs = 0;
            sumNrOfJobs2 = 0;
            sumTime = 0;

            Writer?.WriteLine($"Current Simulation Time,Average number of jobs, StDev on average number of jobs");
        }

        protected override void OnUpdate(ModelElementBase modelElement)
        {
            ServerPool serverPool = (ServerPool)modelElement;
            currentTime = serverPool.GetTime;
            sumNrOfJobs += (currentTime - oldTime) * serverPool.JobCount;
            sumNrOfJobs2 += (currentTime - oldTime) * Math.Pow(serverPool.JobCount, 2);
            sumTime += (currentTime - oldTime);

            averageNrOfJobs = sumNrOfJobs / sumTime;
            averageNrOfJobs2 = sumNrOfJobs2 / sumTime;

            sigmaNrOfJobs = averageNrOfJobs2 - Math.Pow(averageNrOfJobs, 2);

            oldTime = currentTime;

            Writer.WriteLine($"{currentTime}',{averageNrOfJobs},{sigmaNrOfJobs}");
        }

        protected override void OnWarmUp(ModelElementBase modelElement)
        {
        }

        protected override void OnReplicationStart(ModelElementBase modelElement)
        {
        }
    }
}
