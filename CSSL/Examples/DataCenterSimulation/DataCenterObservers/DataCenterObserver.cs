using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Observer;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.DataCenterSimulation.DataCenterObservers
{
    public class DataCenterObserver : ModelElementObserverBase
    {
        public DataCenterObserver(Simulation mySimulation) : base(mySimulation)
        {
        }

        private int totalJobCount;

        protected sealed override void OnInitialized(ModelElementBase modelElement)
        {
            DataCenter dataCenter = (DataCenter)modelElement;
            totalJobCount = dataCenter.Dispatcher.TotalNrJobsInSystem;

            Writer.WriteLine($"Simulation Time,Wall Clock Time,Job Count");
        }

        protected override void OnWarmUp(ModelElementBase modelElement)
        {
        }

        protected sealed override void OnUpdate(ModelElementBase modelElement)
        {
            DataCenter dataCenter = (DataCenter)modelElement;
            totalJobCount = dataCenter.Dispatcher.TotalNrJobsInSystem;

            Writer.WriteLine($"{dataCenter.GetTime},{dataCenter.GetWallClockTime},{totalJobCount}");
            //Console.WriteLine($"{simulationTime}\t{computationalTime}\t{totalJobCount}");
        }

        protected override void OnReplicationStart(ModelElementBase modelElement)
        {
        }

        protected override void OnReplicationEnd(ModelElementBase modelElement)
        {
        }

        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        protected override void OnExperimentStart(ModelElementBase modelElement)
        {
        }

        protected override void OnExperimentEnd(ModelElementBase modelElement)
        {
        }
    }
}
