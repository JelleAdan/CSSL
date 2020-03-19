using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Observer;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.DataCenterSimulation.DataCenterObservers
{
    public class JobCountObserver : ModelElementObserverBase
    {
        public int JobCount;

        public JobCountObserver(Simulation mySimulation) : base(mySimulation)
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
            Writer.WriteLine("Computational Time\tSimulation Time\t");
        }

        protected override void OnUpdate(ModelElementBase modelElement)
        {
            JobCount++;

            Writer.WriteLine($"{modelElement.GetWallClockTime}");
        }

        protected override void OnReplicationStart(ModelElementBase modelElement)
        {
        }

        protected override void OnWarmUp(ModelElementBase modelElement)
        {
            throw new NotImplementedException();
        }
    }
}
