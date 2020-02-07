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

        private double computationalTime;

        private double simulationTime;


        protected sealed override void OnInitialized(ModelElementBase modelElement)
        {
            DataCenter dataCenter = (DataCenter)modelElement.Parent;
            totalJobCount = dataCenter.Dispatcher.TotalNrJobsInSystem;
            computationalTime = dataCenter.Dispatcher.GetElapsedWallClockTime;
            simulationTime = dataCenter.Dispatcher.GetElapsedSimulationClockTime;


            Writer.WriteLine("Simulation Time\tComputational Time\tJob Count");
        }

        protected override void OnWarmUp(ModelElementBase modelElement)
        {
        }

        protected sealed override void OnUpdate(ModelElementBase modelElement)
        {
            DataCenter dataCenter = (DataCenter)modelElement.Parent;
            totalJobCount = dataCenter.Dispatcher.TotalNrJobsInSystem;
            computationalTime = dataCenter.Dispatcher.GetElapsedWallClockTime;
            simulationTime = dataCenter.Dispatcher.GetElapsedSimulationClockTime;

            Writer.WriteLine($"{simulationTime}\t{computationalTime}\t{totalJobCount}");
            Console.WriteLine($"{simulationTime}\t{computationalTime}\t{totalJobCount}");
        }

        protected override void OnReplicationStart(ModelElementBase modelElement)
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
    }
}
