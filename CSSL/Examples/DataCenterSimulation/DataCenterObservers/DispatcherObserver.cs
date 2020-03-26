using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Observer;
using CSSL.Utilities.Statistics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.DataCenterSimulation.DataCenterObservers
{
    public class DispatcherObserver : ModelElementObserverBase
    {
        public DispatcherObserver(Simulation mySimulation) : base(mySimulation)
        {
            queueLength = new Variable<int>(this);
            queueLengthStatistic = new WeightedStatistic("Dispatcher queue length");
        }

        private Variable<int> queueLength;

        private WeightedStatistic queueLengthStatistic;

        protected override void OnExperimentStart(ModelElementBase modelElement)
        {
        }

        protected override void OnReplicationStart(ModelElementBase modelElement)
        {
            queueLength.Reset();
            queueLengthStatistic.Reset();
        }

        protected override void OnWarmUp(ModelElementBase modelElement)
        {
        }

        protected override void OnInitialized(ModelElementBase modelElement)
        {
            queueLength.UpdateValue(queueLength.Value);
            Writer.WriteLine($"Current Simulation Time,Current queue length");
        }

        protected override void OnUpdate(ModelElementBase modelElement)
        {
            Dispatcher dispatcher = (Dispatcher)modelElement;
            queueLength.UpdateValue(dispatcher.QueueLength);
            queueLengthStatistic.Collect(queueLength.Value, queueLength.Weight);

            Writer.WriteLine($"{dispatcher.GetTime},{queueLength.Value}");
        }

        protected override void OnReplicationEnd(ModelElementBase modelElement)
        {
            Writer.WriteLine($"Average queue length,Standard Deviation on queue length");
            Writer.WriteLine($"{queueLengthStatistic.Average()},{queueLengthStatistic.StandardDeviation()}");
        }

        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        protected override void OnExperimentEnd(ModelElementBase modelElement)
        {
        }
    }
}
