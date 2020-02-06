using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Modeling.Elements.Variables;
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
            queueLengthStatistic = new WeightedStatistic("Dispatcher queue length");
        }

        private WeightedStatistic queueLengthStatistic;

        protected override void OnUpdate(ModelElementBase modelElement)
        {
            Variable<int> queueLength = (Variable<int>)modelElement;
            queueLengthStatistic.Collect(queueLength.Value);
        }

        protected override void OnWarmUp(ModelElementBase modelElement)
        {
            throw new NotImplementedException();
        }

        protected override void OnInitialized(ModelElementBase modelElement)
        {
            throw new NotImplementedException();
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
