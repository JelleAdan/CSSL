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


        protected sealed override void OnInitialized(ModelElementBase modelElement)
        {
            DataCenter dataCenter = (DataCenter)modelElement.Parent;
            totalJobCount = dataCenter.Dispatcher.TotalNrJobsInSystem;
            computationalTime = dataCenter.Dispatcher.GetComputationalTime;

            Writer.WriteLine("Computational Time,Job Count");
        }

        protected sealed override void OnUpdate(ModelElementBase modelElement)
        {
            DataCenter dataCenter = (DataCenter)modelElement.Parent;
            totalJobCount = dataCenter.Dispatcher.TotalNrJobsInSystem;
            computationalTime = dataCenter.Dispatcher.GetComputationalTime;

            Writer.WriteLine($"{computationalTime},{totalJobCount}");
        }

        protected override void OnWarmUp(ModelElementBase modelElement)
        {
        }
    }
}
