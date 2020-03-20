using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Observer;
using CSSL.Utilities.Statistics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.DataCenterSimulation.DataCenterObservers
{
    public class DispatcherQueueLengthObserver : ModelElementObserverBase
    {
        public DispatcherQueueLengthObserver(Simulation mySimulation) : base(mySimulation)
        {
            queueLength = new Variable<int>(this);
            queueLengthStatistic = new WeightedStatistic("Dispatcher queue length");
        }

        private Variable<int> queueLength;

        private WeightedStatistic queueLengthStatistic;

        protected override void OnUpdate(ModelElementBase modelElement)
        {
            Dispatcher dispatcher = (Dispatcher)modelElement;
            queueLength.UpdateValue(dispatcher.QueueLength);
            queueLengthStatistic.Collect(queueLength.PreviousValue, queueLength.Weight);
        }

        protected override void OnWarmUp(ModelElementBase modelElement)
        {
        }

        protected override void OnInitialized(ModelElementBase modelElement)
        {
            queueLength.UpdateValue(queueLength.Value);
        }

        protected override void OnReplicationStart(ModelElementBase modelElement)
        {
            queueLength.Reset();
        }

        protected override void OnReplicationEnd(ModelElementBase modelElement)
        {
        }

        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }
    }
}
