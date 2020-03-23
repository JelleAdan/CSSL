using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Observer;
using CSSL.Utilities.Statistics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.WaferFab.Observers
{
    public class QueueObserver : ModelElementObserverBase
    {
        public QueueObserver(Simulation mySimulation) : base(mySimulation)
        {
            queueLength = new Variable<int>(this);
            queueLengthStatistic = new WeightedStatistic("QueueLength");
        }

        private Variable<int> queueLength;

        private WeightedStatistic queueLengthStatistic;

        protected override void OnUpdate(ModelElementBase modelElement)
        {
            throw new NotImplementedException();
        }

        protected override void OnWarmUp(ModelElementBase modelElement)
        {
            throw new NotImplementedException();
        }

        protected override void OnInitialized(ModelElementBase modelElement)
        {
            throw new NotImplementedException();
        }

        protected override void OnReplicationStart(ModelElementBase modelElement)
        {
            throw new NotImplementedException();
        }

        protected override void OnReplicationEnd(ModelElementBase modelElement)
        {
            throw new NotImplementedException();
        }

        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }
    }
}
