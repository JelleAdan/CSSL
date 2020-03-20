using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Observer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSSL.Examples.DataCenterSimulation.DataCenterObservers
{
    public class DispatcherObserver : ModelElementObserverBase
    {
        public DispatcherObserver(Simulation sim) : base(sim)
        {
        }

        private double sumQueueLength;

        private double sumTime;

        private double oldTime;

        private double currentTime;

        protected sealed override void OnUpdate(ModelElementBase modelElement)
        {
            Dispatcher dispatcher = (Dispatcher)modelElement;
            currentTime = dispatcher.GetTime;
            int queueLength = dispatcher.QueueLength;
            sumQueueLength += (currentTime - oldTime) * queueLength;
            sumTime += (currentTime - oldTime);
            oldTime = currentTime;

            Writer.WriteLine($"{currentTime},{sumQueueLength / sumTime}");
            //Console.WriteLine($"Average queue length: {sumQueueLength / sumTime}");
        }

        protected sealed override void OnInitialized(ModelElementBase modelElement)
        {
            oldTime = modelElement.GetTime;

            Writer.WriteLine($"Current simulation time, Average queue length");
        }

        protected sealed override void OnWarmUp(ModelElementBase modelElement)
        {
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
    }
}
